using TechMastery.MarketPlace.Application.Contracts.Identity;
using TechMastery.MarketPlace.Application.Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TechMastery.MarketPlace.Api.Controllers
{
    public class SocialLoginRequest
    {
        public string Token { get; set; }
        public string Provider { get; set; }
    }


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

        [HttpPost("authenticate-social")]
        public async Task<ActionResult<AuthenticationResponse>> AuthenticateSocialAsync([FromBody] SocialLoginRequest request)
        {
            return Ok(await _authenticationService.AuthenticateSocialAsync(request.Token, request.Provider));
        }

    }
}
