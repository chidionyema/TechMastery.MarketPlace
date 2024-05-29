using System.ComponentModel.DataAnnotations;

namespace TechMastery.UsermanagementService.Controllers
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}