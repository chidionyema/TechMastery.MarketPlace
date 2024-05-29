using System;
using Facebook;
using TechMastery.MarketPlace.Application.Contracts.Identity;
using TechMastery.MarketPlace.Application.Models.Authentication;

namespace TechMastery.MarketPlace.Identity.Services
{
    public class FacebookAuthenticator : ISocialAuthenticator
    {
        private readonly FacebookClient _facebookClient;
        private readonly string _appAccessToken; // This should be retrieved from a secure source or configuration.

        public FacebookAuthenticator()
        {
            _facebookClient = new FacebookClient();
            _appAccessToken = "YOUR_APP_ID|YOUR_APP_SECRET";
        }

        public async Task<SocialPayload> ValidateToken(string token)
        {
            // Validate the token using Facebook's debug_token endpoint
            dynamic debugTokenResult = _facebookClient.Get("debug_token", new
            {
                input_token = token,
                access_token = _appAccessToken
            });

            if (debugTokenResult == null || !debugTokenResult.data.is_valid)
            {
                throw new InvalidTokenException("Invalid Facebook token.");
            }

            var userId = debugTokenResult.data.user_id.ToString();

            // Fetch user details using the token
            dynamic userDetails = _facebookClient.Get(userId, new { fields = "id,email", access_token = token });

            if (userDetails == null)
            {
                throw new Exception("Failed to retrieve user details from Facebook.");
            }

            var email = userDetails.email;

            return new FacebookPayload
            {
                Email = email,
                UserId = userId
            };
        }
    }

}

