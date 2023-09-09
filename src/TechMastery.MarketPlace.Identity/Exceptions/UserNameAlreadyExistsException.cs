namespace TechMastery.MarketPlace.Identity.Services
{
    public class UserNameAlreadyExistsException : Exception
    {
        public UserNameAlreadyExistsException(string message) : base(message) { }
    }
}