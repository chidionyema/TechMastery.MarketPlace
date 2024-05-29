using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechMastery.UsermanagementService;
[ApiController]
[Route("[controller]")]
public class SocialLoginController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SocialLoginController> _logger;

    public SocialLoginController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        IJwtTokenService jwtTokenService,
        ILogger<SocialLoginController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    [HttpGet("external-login/{provider}")]
    public IActionResult ExternalLogin(string provider, string returnUrl = null, string clientId = null)
    {
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "SocialLogin", new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        if (!string.IsNullOrEmpty(clientId))
        {
            // Store the clientId in the AuthenticationProperties
            properties.Items["clientId"] = clientId;
        }
        _logger.LogInformation("Redirecting to external provider {Provider}", provider);
        return Challenge(properties, provider);
    }

    [HttpGet("external-login-callback")]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
    {
        if (remoteError != null)
        {
            _logger.LogWarning("Error from external provider: {RemoteError}", remoteError);
            return BadRequest($"Error from external provider: {remoteError}");
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            _logger.LogError("Error loading external login information.");
            return BadRequest("Error loading external login information.");
        }

        // Check if AuthenticationProperties is null before accessing .Items
        if (info.AuthenticationProperties == null)
        {
            _logger.LogError("AuthenticationProperties is null.");
            return BadRequest("An unexpected error occurred. Please try again.");
        }

        if (!info.AuthenticationProperties.Items.TryGetValue("clientId", out var clientId))
        {
            _logger.LogWarning("clientId is missing from the external login process.");
            return BadRequest("Client identification is missing from the request.");
        }

        // Validate clientId
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

        var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        if (signInResult.Succeeded)
        {
            _logger.LogInformation("User logged in with {Provider} provider.", info.LoginProvider);
            return await GenerateTokenAndUpdateAuthTokens(info, clientId);
        }
        else
        {
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            // Verify the email is not already in use
            if (await _userManager.FindByEmailAsync(email) != null)
            {
                _logger.LogWarning("Email {Email} is already associated with another account.", email);
                return BadRequest("Email is already associated with another account.");
            }

            return await HandleUserRegistrationAndLogin(info, clientId);
        }
    }

    private async Task<IActionResult> GenerateTokenAndUpdateAuthTokens(ExternalLoginInfo info, string clientId)
    {
        await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
        var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
        if (user == null)
        {
            _logger.LogError("User not found for provider {Provider} with key {ProviderKey}.", info.LoginProvider, info.ProviderKey);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Message = "An error occurred while retrieving user information." });
        }

        // Now, pass the clientId to the token generation service
        var tokenString = _jwtTokenService.GenerateToken(user, clientId);
        _logger.LogInformation("Generated JWT token for user {UserId} with clientId {ClientId}.", user.Id, clientId);
        return Ok(new { Token = tokenString, Username = user.UserName });
    }


    private async Task<IActionResult> HandleUserRegistrationAndLogin(ExternalLoginInfo info, string clientId)
    {
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var user = new ApplicationUser { UserName = email, Email = email };
        var createUserResult = await _userManager.CreateAsync(user);
        if (!createUserResult.Succeeded)
        {
            _logger.LogError("Failed to create user for email {Email}.", email);
            return BadRequest(createUserResult.Errors);
        }

        var addLoginResult = await _userManager.AddLoginAsync(user, info);
        if (!addLoginResult.Succeeded)
        {
            _logger.LogError("Failed to add external login for user {UserId}.", user.Id);
            return BadRequest(addLoginResult.Errors);
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        var tokenString = _jwtTokenService.GenerateToken(user, clientId);
        _logger.LogInformation("New user registered and signed in with {Provider}.", info.LoginProvider);
        return Ok(new { Token = tokenString, Username = user.UserName });
    }
}
