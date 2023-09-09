using TechMastery.MarketPlace.Application.Models.Mail;
using System.Threading.Tasks;

namespace TechMastery.MarketPlace.Application.Contracts.Infrastructure
{
    public interface IEmailService
    {
        Task SendDownloadLinkEmailAsync(object orderEmail, object sasUrl);
        Task<bool> SendEmail(Email email);
        Task SendPaymentConfirmationEmailAsync(object customerEmail);
    }
}
