using System.ComponentModel.DataAnnotations;
namespace TechMastery.UsermanagementService.Controllers
{
    public class RoleDto {

        [Required]
        public string Name { get; set; } = string.Empty;
    }
 }


