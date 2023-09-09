using System;
namespace TechMastery.MarketPlace.Application.Contracts.Messaging
{
    public interface IMessage
    {
        Guid MessageId { get; }
        DateTime Timestamp { get; }
    }
}

