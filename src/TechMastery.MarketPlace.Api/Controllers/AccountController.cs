using TechMastery.MarketPlace.Application.Contracts.Identity;
using TechMastery.MarketPlace.Application.Models.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace TechMastery.MarketPlace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        public AccountController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request)
        {
            return Ok(await _authenticationService.AuthenticateAsync(request));
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegistrationResponse>> RegisterAsync(RegistrationRequest request)
        {
            return Ok(await _authenticationService.RegisterAsync(request));
        }

       // [HttpPost("external-login-google")]
       // public IActionResult ExternalLoginGoogle()
        //{
           // var authenticationProperties = _authenticationService.ConfigureExternalAuthenticationProperties("Google", "YOUR_CALLBACK_URL");
          //  return Challenge(authenticationProperties, "Google");
        //}

    }
}
