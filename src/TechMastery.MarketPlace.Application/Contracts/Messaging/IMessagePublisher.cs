using System;
namespace TechMastery.MarketPlace.Application.Contracts.Messaging
{
	public interface IMessagePublisher
	{
        Task PublishAsync<TMessage>(TMessage message, string queueName) where TMessage : class, IMessage;
        Task PublishSerializedAsync(string serializedMessage, string queueName);
        public void Dispose();
    }
}

