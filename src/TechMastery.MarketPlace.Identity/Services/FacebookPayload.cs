using System;
using TechMastery.MarketPlace.Application.Models.Authentication;

namespace TechMastery.MarketPlace.Identity.Services
{
    public class FacebookPayload : SocialPayload
    {
        public string Email { get; set; }  // If you have the email permission
                                           // ... any other fields you need
    }
}

