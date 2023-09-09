using System;
using TechMastery.MarketPlace.Application.Models.Authentication;

namespace TechMastery.MarketPlace.Identity.Services
{
    public class GooglePayload : SocialPayload
    {
        public string Sub { get; set; }  // The user's unique Google ID
        public string Email { get; set; }
        public string Subject { get; internal set; }
        // ... any other fields you need
    }

}

