using System.ComponentModel.DataAnnotations;

namespace TechMastery.UsermanagementService.Controllers
{
    public class ResetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$", ErrorMessage = "The password must have at least one uppercase, one lowercase, and one number.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}