

namespace TechMastery.MarketPlace.Application.Contracts
{
    public interface IEmailService
    {
        Task SendDownloadLinkEmailAsync(string orderEmail, string sasUrl);
        Task SendPaymentConfirmationEmailAsync(string customerEmail);
    }
}
