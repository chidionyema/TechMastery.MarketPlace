using TechMastery.MarketPlace.Application.Models.Authentication;

namespace TechMastery.MarketPlace.Application.Contracts.Identity
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request);
        Task<AuthenticationResponse> AuthenticateSocialAsync(string token, string provider);
        Task<RegistrationResponse> RegisterAsync(RegistrationRequest request);
        Task RequestPasswordResetAsync(string email);
    }
}
