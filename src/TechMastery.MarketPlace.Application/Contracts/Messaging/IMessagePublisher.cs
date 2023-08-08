using System;
namespace TechMastery.MarketPlace.Application.Contracts.Messaging
{
	public interface IMessagePublisher
	{
        Task PublishAsync<TMessage>(TMessage message, string queueName) where TMessage : class, IMessage;

        public void Dispose();
    }
}

