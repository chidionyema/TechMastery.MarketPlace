namespace TechMastery.UsermanagementService.Controllers
{
    using System.ComponentModel.DataAnnotations;

    public class UserRegistrationDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string ClientId { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The password must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$", ErrorMessage = "The password must have at least one uppercase, one lowercase, and one number.")]
        public string Password { get; set; } = string.Empty;
    }

}