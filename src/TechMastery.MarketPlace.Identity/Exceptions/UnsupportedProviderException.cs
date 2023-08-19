namespace TechMastery.MarketPlace.Identity.Services
{
    public class UnsupportedProviderException : Exception
    {
        public UnsupportedProviderException() { }

        public UnsupportedProviderException(string provider)
            : base($"Unsupported authentication provider: {provider}.")
        {
        }

        public UnsupportedProviderException(string provider, Exception inner)
            : base($"Unsupported authentication provider: {provider}.", inner)
        {
        }
    }
}