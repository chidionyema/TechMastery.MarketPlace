using MassTransit;
using Microsoft.Extensions.Logging;
using TechMastery.MarketPlace.Application.Contracts.Infrastructure;
using TechMastery.MarketPlace.Application.Messages.Payment;
using TechMastery.MarketPlace.Application.Persistence.Contracts;
using TechMastery.MarketPlace.Domain.Entities;

public class PaymentInitiatedConsumer : IConsumer<PaymentInitiatedEvent>
{
    private readonly IPaymentService _paymentService;
    private readonly ISalesTransactionRepository _salesTransactionRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<PaymentInitiatedConsumer> _logger;

    private const int PAYMENT_PROCESS_TIMEOUT_MINUTES = 5; // Consider moving this to a config file

    public PaymentInitiatedConsumer () { }

    public PaymentInitiatedConsumer(
        IPaymentService paymentService,
        ISalesTransactionRepository salesTransactionRepository,
        ILogger<PaymentInitiatedConsumer> logger,
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository)
    {
        _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        _salesTransactionRepository = salesTransactionRepository ?? throw new ArgumentNullException(nameof(salesTransactionRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
    }

    public async Task Consume(ConsumeContext<PaymentInitiatedEvent> context)
    {
        var paymentEvent = context.Message;

        _logger.LogInformation($"Processing payment for Order ID: {paymentEvent.OrderId}");

        var order = await _orderRepository.GetByIdAsync(paymentEvent.OrderId);
        if (order == null)
        {
            _logger.LogError($"Order with ID {paymentEvent.OrderId} not found");
            return;
        }

        if (order.OrderTotal != paymentEvent.PaymentAmount)
        {
            _logger.LogWarning($"Payment amount mismatch for Order ID {paymentEvent.OrderId}. Expected {order.OrderTotal}, but got {paymentEvent.PaymentAmount}");
            return;
        }

        var existingPayment = await _paymentRepository.GetSuccessfulPaymentByOrderId(order.Id);
        if (existingPayment != null)
        {
            _logger.LogWarning($"Duplicate payment attempt detected for Order ID {paymentEvent.OrderId}");
            return;
        }
        order.SetOrderStatus(OrderStatus.InProgress);
        await _orderRepository.UpdateAsync(order);

        var paymentRecord = new Payment(order.Id, order.OrderTotal);
        await _paymentRepository.AddAsync(paymentRecord);

        try
        {
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(PAYMENT_PROCESS_TIMEOUT_MINUTES)).Token;
            var paymentResult = await _paymentService.ProcessPaymentAsync(paymentEvent.PaymentInfo, cancellationToken);

            if (paymentResult.IsSuccess)
            {
                paymentRecord.MarkSuccess(paymentResult.PaymentId);
                await _paymentRepository.UpdateAsync(paymentRecord);

                var salesRecord = ConvertOrderToSaleTransaction(order, paymentResult.PaymentId);
                await _salesTransactionRepository.AddAsync(salesRecord);

                await context.Publish(new PaymentCompletedEvent { OrderId = order.Id });
            }
            else
            {
                paymentRecord.MarkFailed(paymentResult.ErrorMessage);
                await _paymentRepository.UpdateAsync(paymentRecord);
                _logger.LogError($"Payment processing failed for OrderId: {order.Id}. Error: {paymentResult.ErrorMessage}");
                await context.Publish(new PaymentFailedEvent { OrderId = order.Id, ErrorMessage = paymentResult.ErrorMessage });
            }
        }
        catch (Exception ex)
        {
            paymentRecord.MarkFailed("Internal error occurred");  // Generic error
            await _paymentRepository.UpdateAsync(paymentRecord);

            _logger.LogError(ex, $"An error occurred while processing payment for OrderId: {order.Id}");
            await context.Publish(new PaymentFailedEvent { OrderId = order.Id, ErrorMessage = "An error occurred while processing the payment." });

        }
    }

    private SaleTransaction ConvertOrderToSaleTransaction(Order order, string paymentId)
    {
        return new SaleTransaction(order.Id, DateTime.UtcNow, order.OrderTotal, paymentId);
    }
}
