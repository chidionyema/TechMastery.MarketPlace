
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TechMastery.UsermanagementService
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string GoogleId { get; set; } = string.Empty;
        public string FacebookId { get; set; } = string.Empty;
    }

    public class Client
    {
        [Key]
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecretHash { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string LogoutUri { get; set; } = string.Empty;
    }


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Client> Clients { get; set; }
        public ApplicationDbContext()
        {

        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}

