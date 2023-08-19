using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TechMastery.MarketPlace.Application.Models.Authentication;
using TechMastery.MarketPlace.Application.Contracts.Identity;

namespace TechMastery.MarketPlace.Identity.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly Dictionary<string, ISocialAuthenticator> _socialAuthenticators;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            IOptions<JwtSettings> jwtSettings,
            SignInManager<ApplicationUser> signInManager,
            IEnumerable<ISocialAuthenticator> socialAuthenticators,
            ILogger<AuthenticationService> logger)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _signInManager = signInManager;
            _logger = logger;
            _socialAuthenticators = socialAuthenticators.ToDictionary(
            auth => auth.GetType().Name.Replace("Authenticator", "").ToLower(),
            auth => auth);
        }

        public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning($"User with email: {request.Email} not found.");
                throw new UserNotFoundException($"User with email {request.Email} not found.");
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                _logger.LogWarning($"Invalid credentials for email: {request.Email}");
                throw new InvalidCredentialsException($"Credentials for '{request.Email}' aren't valid.");
            }

            return await GenerateAuthenticationResponseForUser(user);
        }

        public async Task<RegistrationResponse> RegisterAsync(RegistrationRequest request)
        {
            if (await _userManager.FindByNameAsync(request.UserName) != null)
            {
                _logger.LogWarning($"Username {request.UserName} already exists.");
                throw new UserNameAlreadyExistsException($"Username '{request.UserName}' already exists.");
            }

            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                _logger.LogWarning($"Email {request.Email} already exists.");
                throw new EmailAlreadyExistsException($"Email {request.Email} already exists.");
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                _logger.LogError($"Error during registration for email: {request.Email}");
                throw new RegistrationFailedException($"Error during registration. {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            return new RegistrationResponse() { UserId = user.Id };
        }

        public async Task<IdentityResult> AddLoginAsync(ApplicationUser user, ExternalLoginInfo info)
        {
            return await _userManager.AddLoginAsync(user, info);
        }

        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning($"User with email: {email} not found.");
                throw new UserNotFoundException($"User with email {email} not found.");
            }
            return user;
        }

        public async Task<AuthenticationResponse> AuthenticateSocialAsync(string token, string provider)
        {
            if (!_socialAuthenticators.ContainsKey(provider.ToLower()))
            {
                _logger.LogError($"Unsupported provider: {provider}");
                throw new UnsupportedProviderException($"Unsupported provider: {provider}");
            }

            var payload = await _socialAuthenticators[provider.ToLower()].ValidateToken(token);
            var user = await ConnectOrCreateLocalAccount(payload, provider);

            return await GenerateAuthenticationResponseForUser(user);
        }


        private async Task<ApplicationUser> ConnectOrCreateLocalAccount(object payload, string provider)
        {
            string email;
            string id;

            switch (provider)
            {
                case "Google":
                    var googlePayload = payload as GooglePayload;
                    email = googlePayload.Email;
                    id = googlePayload.Sub;
                    break;
                case "Facebook":
                    var fbPayload = payload as FacebookPayload;
                    email = fbPayload.Email;
                    id = fbPayload.UserId;
                    break;
                default:
                    throw new ArgumentException($"Unsupported provider: {provider}", nameof(provider));
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true
                };
                var createUserResult = await _userManager.CreateAsync(user);
                if (!createUserResult.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to create a new user: {string.Join(", ", createUserResult.Errors.Select(e => e.Description))}");
                }
            }

            var externalLoginInfo = new ExternalLoginInfo(null, provider, id, provider);
            var addLoginResult = await _userManager.AddLoginAsync(user, externalLoginInfo);
            if (!addLoginResult.Succeeded)
            {
                throw new InvalidOperationException($"Failed to add external login for user {user.UserName}: {string.Join(", ", addLoginResult.Errors.Select(e => e.Description))}");
            }

            return user;
        }

        public Task RequestPasswordResetAsync(string email)
        {
            // TODO: Implement the request password reset logic, sending the reset link to the user's email, etc.
            throw new NotImplementedException();
        }

        private async Task<JwtSecurityToken> GenerateToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim("roles", role));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        private async Task<AuthenticationResponse> GenerateAuthenticationResponseForUser(ApplicationUser user)
        {
            var jwtSecurityToken = await GenerateToken(user);

            return new AuthenticationResponse
            {
                Id = user.Id,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Email = user.Email,
                UserName = user.UserName
            };
        }

    }
}