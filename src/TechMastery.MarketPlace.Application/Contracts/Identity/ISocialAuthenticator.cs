using System;
using TechMastery.MarketPlace.Application.Models.Authentication;

namespace TechMastery.MarketPlace.Application.Contracts.Identity
{
    public interface ISocialAuthenticator
    {
        Task<SocialPayload> ValidateToken(string token);

    }
}

