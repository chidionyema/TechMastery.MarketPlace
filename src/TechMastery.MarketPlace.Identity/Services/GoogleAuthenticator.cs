using System;
using Google.Apis.Auth;
using Microsoft.Extensions.Logging;
using TechMastery.MarketPlace.Application.Contracts.Identity;
using TechMastery.MarketPlace.Application.Models.Authentication;

namespace TechMastery.MarketPlace.Identity.Services
{
    public class GoogleAuthenticator : ISocialAuthenticator
    {
        private readonly ILogger<GoogleAuthenticator> _logger;

        public GoogleAuthenticator(ILogger<GoogleAuthenticator> logger)
        {
            _logger = logger;
        }

        public async Task<GooglePayload> ValidateGoogleToken(string token)
        {
            GooglePayload googlePayload;
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(token);
                googlePayload = new GooglePayload
                {
                    Sub = payload.Subject,
                    Email = payload.Email
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error validating Google token: {ex.Message}");
                throw new InvalidTokenException("Invalid Google token.");
            }

            return googlePayload;
        }

        public Task<SocialPayload> ValidateToken(string token)
        {
            throw new NotImplementedException();
        }
    }

}

