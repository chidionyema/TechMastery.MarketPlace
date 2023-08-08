using System;
using MassTransit;
using TechMastery.Messaging.Consumers.Messages;

namespace TechMastery.Messaging.Consumers.Consumers
{
    public class ProductAddedConsumer : IConsumer<ProductAdded>
    {
        public async Task Consume(ConsumeContext<ProductAdded> context)
        {
            var productAddedMessage = context.Message;
            Console.WriteLine($"ProductAdded - ProductId: {productAddedMessage.ProductId}, ProductName: {productAddedMessage.ProductName}, Price: {productAddedMessage.Price}");
        }
    }
}

