using System.Security.Cryptography;
using System.Text;

namespace TechMastery.UsermanagementService
{
    public static class HashingHelper
    {
        public static string HashSecret(string secret)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(secret));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }
}

