namespace TechMastery.UsermanagementService
{
    public interface IJwtTokenService
    {
        Task<string> GenerateToken(ApplicationUser user, string clientId);
    }
}