using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TechMastery.MarketPlace.Application.Contracts;
namespace TechMastery.MarketPlace.Application.Services
{
    public class LoggedInUserService : ILoggedInUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public LoggedInUserService(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }

        public string UserId
        {
            get
            {
                return _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
        }
    }
}

