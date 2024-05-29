
namespace TechMastery.UsermanagementService.Controllers
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TechMastery.MarketPlace.Application.Contracts;

    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IJwtTokenService jwtTokenService,
            ILogger<AuthenticationController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDto model)
        {
            // Validate clientId
            ValidateClientId(model.ClientId);
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", new { userId = user.Id, token }, Request.Scheme);
                // TODO: Send confirmationLink via email

                return Ok(new { UserId = user.Id, Message = "Registration successful. Please confirm your email." });
            }
            return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("User ID and token are required.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            token = Uri.UnescapeDataString(token); // Ensure the token is properly decoded
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok("Email confirmed successfully.");
            }
            else
            {
                return BadRequest("Email could not be confirmed.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto model)
        {
            // Validate clientId
            ValidateClientId(model.ClientId);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // To prevent account enumeration, you might consider always returning a generic message.
                return Unauthorized("Invalid login attempt.");
            }

           // if (!await _userManager.IsEmailConfirmedAsync(user))
           // {
             //   return Unauthorized("You must have a confirmed email to log in.");
            //}

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var tokenString = _jwtTokenService.GenerateToken(user, model.ClientId);
                return Ok(new { Token = tokenString, Username = user.UserName });
            }
            else if (result.IsLockedOut)
            {
                return Unauthorized("This account has been locked out, please try again later.");
            }
            else
            {
                return Unauthorized ("Invalid login attempt.");
            }
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Even if the user doesn't exist or isn't confirmed, pretend the process is successful to avoid account enumeration.
                return Ok("If your email is in our system, you will receive a password reset email.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action(nameof(ResetPassword), "Authentication", new { token }, Request.Scheme);
            // TODO: Email the reset link to the user

            return Ok("If your email is in our system, you will receive a password reset email.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // To prevent account enumeration, you might consider always returning a successful response.
                return Ok("Your password has been reset successfully.");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok("Your password has been reset successfully.");
            }

            return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
        }


        
        private IActionResult ValidateClientId(string clientId)
        {
            var validClientIdsSection = _configuration.GetSection("ValidClients:ClientIds");
            if (!validClientIdsSection.Exists())
            {
                _logger.LogError("ValidClients:ClientIds section is missing in configuration.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Configuration error.");
            }

            var validClientIds = validClientIdsSection.Get<List<string>>();
            if (!validClientIds.Contains(clientId))
            {
                _logger.LogWarning("Unauthorized clientId: {ClientId}", clientId);
                return Unauthorized("Unauthorized client application.");
            }

            return null; // Indicates clientId is valid
        }
        /*
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest request)
        {
            var validationResult = await ValidateRefreshToken(request.RefreshToken);
            if (!validationResult.Success)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(validationResult.UserId);
            if (user == null)
            {
                return Unauthorized();
            }

            var newAccessToken = GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken();
            // Save the new refresh token with the user in the database

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }*/


    }

}

