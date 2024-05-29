using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using System.Text.Json;
using TechMastery.UsermanagementService;

namespace TechMastery.UserManagement.Tests
{
    public class SocialLoginControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
        private readonly Mock<ILogger<SocialLoginController>> _loggerMock; // Add this line

        private readonly SocialLoginController _controller;

        public SocialLoginControllerTests()
        {
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                _userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(), null, null, null, null);
            _configurationMock = new Mock<IConfiguration>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _loggerMock = new Mock<ILogger<SocialLoginController>>(); // Initialize the logger mock

            _controller = new SocialLoginController(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _configurationMock.Object,
                _jwtTokenServiceMock.Object,
                _loggerMock.Object); // Pass the logger mock to the controller
        }

        [Fact]
        public async Task ExternalLoginCallback_WithError_ReturnsBadRequest()
        {
            // Arrange
            string error = "test error";

            // Act
            var result = await _controller.ExternalLoginCallback(null, error);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ExternalLoginCallback_WithSuccessfulLogin_ReturnsOk()
        {
            // Arrange
            var info = new ExternalLoginInfo(new ClaimsPrincipal(), "provider", "key", "display");
            _signInManagerMock.Setup(x => x.GetExternalLoginInfoAsync(It.IsAny<string>())).ReturnsAsync(info);
            _signInManagerMock.Setup(x => x.ExternalLoginSignInAsync("provider", "key", false, true)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            _userManagerMock.Setup(x => x.FindByLoginAsync("provider", "key")).ReturnsAsync(new ApplicationUser());
            _jwtTokenServiceMock.Setup(x => x.GenerateToken(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync("token");

            // Act
            var result = await _controller.ExternalLoginCallback();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task ExternalLoginCallback_WithNoExternalLoginInfo_ReturnsBadRequest()
        {
            // Arrange
            _signInManagerMock.Setup(x => x.GetExternalLoginInfoAsync(It.IsAny<string>())).ReturnsAsync((ExternalLoginInfo)null);

            // Act
            var result = await _controller.ExternalLoginCallback();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ExternalLoginCallback_CreatesNewUser_IfNotExists()
        {
            // Arrange
            var info = new ExternalLoginInfo(new ClaimsPrincipal(), "provider", "key", "display");
            _signInManagerMock.Setup(x => x.GetExternalLoginInfoAsync(It.IsAny<string>())).ReturnsAsync(info);
            _signInManagerMock.Setup(x => x.ExternalLoginSignInAsync("provider", "key", false, true))
                              .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed); // Simulate failed login indicating no user
            _userManagerMock.Setup(x => x.FindByLoginAsync("provider", "key")).ReturnsAsync((ApplicationUser)null); // User not found
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success); // Simulate successful user creation
            _userManagerMock.Setup(x => x.AddLoginAsync(It.IsAny<ApplicationUser>(), It.IsAny<UserLoginInfo>())).ReturnsAsync(IdentityResult.Success); // Simulate successful login addition
            _jwtTokenServiceMock.Setup(x => x.GenerateToken(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync("token");
            // Act
            var result = await _controller.ExternalLoginCallback();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>()), Times.Once); // Verify that a user is created
        }

        [Fact]
        public async Task ExternalLoginCallback_HandlesFailure_InUserCreationOrLoginAddition()
        {
            // Arrange
            var info = new ExternalLoginInfo(new ClaimsPrincipal(), "provider", "key", "display");
            _signInManagerMock.Setup(x => x.GetExternalLoginInfoAsync(It.IsAny<string>())).ReturnsAsync(info);
            _userManagerMock.Setup(x => x.FindByLoginAsync("provider", "key")).ReturnsAsync((ApplicationUser)null); // User not found
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Failed(new IdentityError[] { new IdentityError { Description = "Creation Failed" } })); // Simulate failed user creation
            _signInManagerMock.Setup(x => x.ExternalLoginSignInAsync("provider", "key", false, true)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            // Act
            var result = await _controller.ExternalLoginCallback();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>()), Times.Once); // Verify that it attempted to create a user
        }


        [Fact]
        public async Task ExternalLoginCallback_VerifiesTokenGeneration_ForSuccessfulLogins()
        {
            // Arrange
            var user = new ApplicationUser { UserName = "testUser", Email = "user@example.com" };
            var info = new ExternalLoginInfo(new ClaimsPrincipal(), "provider", "key", "display");
            _signInManagerMock.Setup(x => x.GetExternalLoginInfoAsync(It.IsAny<string>())).ReturnsAsync(info);
            _signInManagerMock.Setup(x => x.ExternalLoginSignInAsync("provider", "key", false, true)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            _userManagerMock.Setup(x => x.FindByLoginAsync("provider", "key")).ReturnsAsync(user);
            _jwtTokenServiceMock.Setup(x => x.GenerateToken(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync("token");

            // Act
            var result = await _controller.ExternalLoginCallback();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JsonSerializer.Serialize(okResult.Value);
            var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            try
            {
                var token = root.GetProperty("Token").GetString(); // Assuming Token is a direct string.
                Assert.Equal("validToken", token);
            }
            catch (InvalidOperationException)
            {
                // If Token is not a direct string but an object, you might need to navigate deeper.
                // For example, if the Token is within another property:
                var token = root.GetProperty("Token").GetProperty("Result").GetString();
                Assert.Equal("validToken", token);
            }

            // Assuming "Username" is a direct property of the root object and is a string.
            var username = root.GetProperty("Username").GetString();
            Assert.Equal("testUser", username);
            // Ensure the token generation method is called once
            _jwtTokenServiceMock.Verify(x => x.GenerateToken(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);


        }

    }

}