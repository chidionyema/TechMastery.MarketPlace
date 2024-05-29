using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TechMastery.UsermanagementService.Tests
{
    public class JwtTokenServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly JwtTokenService _jwtTokenService;

       
    }
}
