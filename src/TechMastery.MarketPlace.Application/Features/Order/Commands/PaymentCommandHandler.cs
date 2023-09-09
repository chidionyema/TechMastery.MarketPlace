using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TechMastery.MarketPlace.Application.Contracts.Infrastructure;
using TechMastery.MarketPlace.Application.Contracts.Persistence;
using TechMastery.MarketPlace.Application.Exceptions;
using TechMastery.MarketPlace.Application.Messages.Payment;
using TechMastery.MarketPlace.Application.Models.Payment;
using TechMastery.MarketPlace.Domain.Entities;

namespace TechMastery.MarketPlace.Application.Features.Orders.Commands
{
    public class PaymentCommand : IRequest<Unit>
    {
        public required PaymentInfo PaymentInfo { get; set; }
        public Guid OrderId { get; set; }
    }

    public class PaymentCommandHandler : IRequestHandler<PaymentCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOutboxRepository _outboxRepository;
        private readonly ILogger<PaymentCommandHandler> _logger;

        public PaymentCommandHandler(
            IOrderRepository orderRepository,
            IOutboxRepository outboxRepository,
            ILogger<PaymentCommandHandler> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _outboxRepository = outboxRepository ?? throw new ArgumentNullException(nameof(outboxRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Unit> Handle(PaymentCommand command, CancellationToken cancellationToken)
        {
            ValidateCommand(command);

            var order = await GetValidatedOrderAsync(command.OrderId);

            var messageType = typeof(PaymentInitiatedEvent);
            string assemblyQualifiedName = messageType.AssemblyQualifiedName;

            if (string.IsNullOrEmpty(assemblyQualifiedName))
            {
                _logger.LogWarning($"AssemblyQualifiedName is null for type {messageType.Name}. Using default type.");

                assemblyQualifiedName = "DefaultType";
            }

            var outboxMessage = new OutboxMessage
            {
                MessageType = assemblyQualifiedName,
                Payload = JsonSerializer.Serialize(new PaymentInitiatedEvent
                {
                    OrderId = order.OrderId,
                    PaymentInfo = command.PaymentInfo,
                    PaymentAmount = order.OrderTotal
                })
            };

            await _outboxRepository.AddAsync(outboxMessage);
            return Unit.Value; 
        }

        private static void ValidateCommand(PaymentCommand command)
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

            if(order.OrderStatus != OrderStatus.Pending)
            {
                throw new BadRequestException("order has been processed."+ orderId);
            }

            return order;
        }
    }
}
