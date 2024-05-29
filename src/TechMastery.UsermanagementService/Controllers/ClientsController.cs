using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TechMastery.UsermanagementService.Dto;

namespace TechMastery.UsermanagementService.Controllers
{
    [Authorize(Roles = "Admin")] // Ensure only admins can access client management
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterClient([FromBody] ClientRegistrationDto registrationDto)
        {
            var clientExists = await _context.Clients.AnyAsync(c => c.ClientId == registrationDto.ClientId);
            if (clientExists)
            {
                return BadRequest("Client already exists.");
            }

            var client = new Client
            {
                ClientId = registrationDto.ClientId,
                ClientSecretHash = HashingHelper.HashSecret(registrationDto.ClientSecret),
                RedirectUri = registrationDto.RedirectUri
                // Set other properties as needed
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Client registered successfully." });
        }

        public async Task<bool> ValidateClient(string clientId, string clientSecret)
        {
            var client = await _context.Clients.FindAsync(clientId);
            if (client == null) return false;

            // Compare the hashed secret
            var hashedSecret = HashingHelper.HashSecret(clientSecret);
            return client.ClientSecretHash == hashedSecret;
        }

    }
}
