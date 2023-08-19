using System;
using System.Linq;
using MediatR;
using TechMastery.MarketPlace.Application.Contracts.Infrastructure;
using TechMastery.MarketPlace.Application.Contracts.Persistence;
using TechMastery.MarketPlace.Application.Exceptions;
using TechMastery.MarketPlace.Application.Models.Payment;
using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Application.Features.Orders.Commands
{
    public class ProcessOrder : IRequest<PaymentResult>
    {
        public required PaymentInfo PaymentInfo { get; set; }
        public Guid OrderId { get; set; }
    }

    /// <summary>
    /// Handles the process of finalizing the order including payment and sending necessary confirmations.
    /// </summary>
    public class ProcessOrderCommandHandler : IRequestHandler<ProcessOrder, PaymentResult>
    {
        private const decimal FeePercentage = 0.15m;

        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentService _paymentService;
        private readonly IEmailService _emailService;
        private readonly IStorageProvider _storageProvider;

        public ProcessOrderCommandHandler(
            IOrderRepository orderRepository,
            IPaymentService paymentService,
            IEmailService emailService,
            IStorageProvider storageProvider)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _storageProvider = storageProvider ?? throw new ArgumentNullException(nameof(storageProvider));
        }

        public async Task<PaymentResult> Handle(ProcessOrder command, CancellationToken cancellationToken)
        {
            ValidateCommand(command);

            var order = await GetValidatedOrderAsync(command.OrderId);
            AdjustPaymentAmount(command.PaymentInfo, CalculateOrderTotal(order));

            var paymentResult = await ProcessPaymentAsync(command.PaymentInfo, cancellationToken);

            if (paymentResult.IsSuccess)
            {
                await HandleSuccessfulPayment(order);
            }

            return paymentResult;
        }

        private static void ValidateCommand(ProcessOrder command)
        {
            if (command == null || command.OrderId == Guid.Empty || command.PaymentInfo == null)
            {
                throw new BadRequestException("Invalid command details.");
            }
        }

        private async Task<Order> GetValidatedOrderAsync(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new NotFoundException($"Order with ID {orderId} not found.", orderId);
            }

            return order;
        }

        private void AdjustPaymentAmount(PaymentInfo paymentInfo, decimal orderTotal)
        {
            decimal fee = orderTotal * FeePercentage;
            paymentInfo.SellerCut -= fee;
        }

        private static decimal CalculateOrderTotal(Order order) => order.OrderLineItems.Sum(item => item.UnitPrice * item.Quantity);

        private async Task<PaymentResult> ProcessPaymentAsync(PaymentInfo paymentInfo, CancellationToken cancellationToken)
        {
            return await _paymentService.ProcessPaymentAsync(paymentInfo, cancellationToken);
        }

        private async Task HandleSuccessfulPayment(Order order)
        {
            await SendPaymentConfirmationEmailAsync(order);
            await SendProductDownloadLinksAsync(order);
        }

        private async Task SendPaymentConfirmationEmailAsync(Order order)
        {
            await _emailService.SendPaymentConfirmationEmailAsync(order.OrderEmail!);
        }

        private async Task SendProductDownloadLinksAsync(Order order)
        {
            var sasExpiryTime = DateTimeOffset.UtcNow.AddHours(1);
            foreach (var orderLineItem in order.OrderLineItems)
            {
                var sasUrl = await GenerateSasDownloadUriForItem(orderLineItem, sasExpiryTime);
                await _emailService.SendDownloadLinkEmailAsync(order.OrderEmail, sasUrl);
            }
        }

        private async Task<Uri> GenerateSasDownloadUriForItem(OrderLineItem orderLineItem, DateTimeOffset sasExpiryTime)
        {
            var blobName = orderLineItem.GetProductArtifactBlobName();
            return await _storageProvider.GenerateSasDownloadUriAsync(blobName, sasExpiryTime);
        }
    }
}
