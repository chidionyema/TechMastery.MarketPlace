using TechMastery.MarketPlace.Application.Contracts;

namespace TechMastery.MarketPlace.Application.Messaging
{
    public class NewUserRegistration : IMessage
    {
        public Guid MessageId { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
 
    }
}

