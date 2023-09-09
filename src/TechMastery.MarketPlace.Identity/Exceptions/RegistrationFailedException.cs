namespace TechMastery.MarketPlace.Identity.Services
{
    public class RegistrationFailedException : Exception
    {
        public RegistrationFailedException(string message) : base(message) { }
    }
}