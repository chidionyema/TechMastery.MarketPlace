using System.ComponentModel.DataAnnotations;

namespace TechMastery.UsermanagementService.Controllers {

    public class RoleAssignmentDto {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string RoleName { get; set; } = string.Empty;
       }
  }


