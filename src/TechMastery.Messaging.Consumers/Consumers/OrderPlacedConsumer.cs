using System;
using MassTransit;
using TechMastery.Messaging.Consumers.Messages;

namespace TechMastery.Messaging.Consumers.Consumers
{
    public class OrderPlacedConsumer : IConsumer<OrderPlaced>
    {
        public async Task Consume(ConsumeContext<OrderPlaced> context)
        {
            var orderPlacedMessage = context.Message;
            Console.WriteLine($"OrderPlaced - OrderId: {orderPlacedMessage.OrderId}, CustomerId: {orderPlacedMessage.CustomerId}, TotalAmount: {orderPlacedMessage.TotalAmount}");
        }
    }
}

