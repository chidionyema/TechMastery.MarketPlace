using MassTransit;
using Microsoft.Extensions.Logging;
using Polly;
using TechMastery.MarketPlace.Application.Contracts;
using TechMastery.MarketPlace.Application.Contracts.Infrastructure;
using TechMastery.MarketPlace.Application.Messages.Payment;
using TechMastery.MarketPlace.Application.Persistence.Contracts;
using TechMastery.MarketPlace.Domain.Entities;


public class PaymentCompletedConsumer : IConsumer<PaymentCompletedEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<PaymentCompletedConsumer> _logger;
    private readonly IStorageProvider _storageProvider;
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;

    private const int SAS_EXPIRY_HOURS = 1;
    private const int MAX_RETRIES = 3;

    public PaymentCompletedConsumer() { }

    public PaymentCompletedConsumer(
        IEmailService emailService,
        ILogger<PaymentCompletedConsumer> logger,
        IStorageProvider storageProvider,
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository)
    {
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _storageProvider = storageProvider ?? throw new ArgumentNullException(nameof(storageProvider));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
    }

    public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
    {
        var paymentCompletedEvent = context.Message;

        _logger.LogInformation($"Processing PaymentCompletedEvent for Order ID: {paymentCompletedEvent.OrderId}");

        try
        {
            var existingPayment = await _paymentRepository.GetByIdAsync(paymentCompletedEvent.PaymentId);
            if (existingPayment == null || existingPayment.Status != PaymentStatusEnum.Successful)
            {
                _logger.LogWarning("PaymentCompletedEvent already processed for Payment ID: {PaymentId}", paymentCompletedEvent.PaymentId);
                return;
            }

            var order = await _orderRepository.GetByIdAsync(paymentCompletedEvent.OrderId);
            if (order == null)
            {
                _logger.LogError("PaymentCompletedEvent received for unknown order ID: {OrderId}", paymentCompletedEvent.OrderId);
                return;
            }

            if (string.IsNullOrEmpty(order.OrderEmail))
            {
                _logger.LogError("OrderEmail is null for Order ID: {OrderId}", paymentCompletedEvent.OrderId);
                return;
            }

            await Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(MAX_RETRIES, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                .ExecuteAsync(async () =>
                {
                    await SendPaymentConfirmationEmailAsync(order.OrderEmail);
                    await SendProductDownloadLinksAsync(order);
                });

            existingPayment.MarkAsCompleted();
            await _paymentRepository.UpdateAsync(existingPayment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while handling PaymentCompletedEvent for Order ID: {OrderId}", paymentCompletedEvent.OrderId);
        }
    }

    private async Task SendPaymentConfirmationEmailAsync(string orderEmail)
    {
        await _emailService.SendPaymentConfirmationEmailAsync(orderEmail);
    }

    private async Task SendProductDownloadLinksAsync(Order order)
    {
        var sasExpiryTime = DateTimeOffset.UtcNow.AddHours(SAS_EXPIRY_HOURS);
        foreach (var orderLineItem in order.OrderLineItems)
        {
            var sasUrl = await GenerateSasDownloadUriForItem(orderLineItem, sasExpiryTime);
            await _emailService.SendDownloadLinkEmailAsync(order.OrderEmail, sasUrl.AbsoluteUri);
        }
    }

    private async Task<Uri> GenerateSasDownloadUriForItem(OrderLineItem orderLineItem, DateTimeOffset sasExpiryTime)
    {
        var blobName = orderLineItem.GetProductArtifactBlobName();
        return await _storageProvider.GenerateSasDownloadUriAsync(blobName, sasExpiryTime);
    }
}
