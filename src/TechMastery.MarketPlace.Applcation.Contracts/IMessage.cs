
namespace TechMastery.MarketPlace.Application.Contracts
{

    public interface IMessage
    {
        Guid MessageId { get; }
        DateTime Timestamp { get; }
    }

}