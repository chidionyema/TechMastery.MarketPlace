using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace TechMastery.UsermanagementService.Controllers
{

        [Authorize(Roles = "Admin")] // Ensure only admins can access role management
        [Route("api/[controller]")]
        [ApiController]
        public class RolesController : ControllerBase
        {
            private readonly RoleManager<IdentityRole> _roleManager;
            private readonly UserManager<ApplicationUser> _userManager;

            public RolesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
            {
                _roleManager = roleManager;
                _userManager = userManager;
            }

            [HttpPost]
            public async Task<IActionResult> CreateRole([FromBody] RoleDto model)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var roleExist = await _roleManager.RoleExistsAsync(model.Name);
                if (roleExist)
                {
                    return BadRequest("Role already exists.");
                }

                var result = await _roleManager.CreateAsync(new IdentityRole(model.Name));
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                return StatusCode(201); // 201 Created
            }

            [HttpDelete("{roleName}")]
            public async Task<IActionResult> DeleteRole(string roleName)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    return NotFound("Role not found.");
                }

                var result = await _roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                return Ok();
            }

            [HttpPost("Assign")]
            public async Task<IActionResult> AssignRole([FromBody] RoleAssignmentDto model)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                var result = await _userManager.AddToRoleAsync(user, model.RoleName);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                return Ok();
            }
        }
    }


