namespace TechMastery.MarketPlace.Identity.Services
{
    // Custom exceptions:
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException()
            : base("Invalid token.")
        {
        }

        public InvalidTokenException(string message)
            : base(message)
        {
        }

        public InvalidTokenException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}