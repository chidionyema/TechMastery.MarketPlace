using Microsoft.AspNetCore.Identity;

namespace TechMastery.MarketPlace.Application.Models.Authentication
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string GoogleId { get; set; } = string.Empty;
        public string FacebookId { get; set; } = string.Empty;
    }
}
