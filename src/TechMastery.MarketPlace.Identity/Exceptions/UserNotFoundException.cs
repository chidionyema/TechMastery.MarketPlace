namespace TechMastery.MarketPlace.Identity.Services
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string message) : base(message) { }
    }
}