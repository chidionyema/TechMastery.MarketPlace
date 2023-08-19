namespace TechMastery.MarketPlace.Identity.Services
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException(string message) : base(message) { }
    }
}