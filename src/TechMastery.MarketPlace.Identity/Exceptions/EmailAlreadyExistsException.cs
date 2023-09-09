namespace TechMastery.MarketPlace.Identity.Services
{
    public class EmailAlreadyExistsException : Exception
    {
        public EmailAlreadyExistsException(string message) : base(message) { }
    }
}