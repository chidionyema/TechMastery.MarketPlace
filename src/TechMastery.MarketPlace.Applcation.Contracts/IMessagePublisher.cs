
namespace TechMastery.MarketPlace.Application.Contracts;

public interface IMessagePublisher
{
    Task PublishAsync<TMessage>(TMessage message, string queueName, CancellationToken cancellationToken) where TMessage : class, IMessage;
    Task PublishSerializedAsync(string serializedMessage, string queueName);
    public void Dispose();
}
