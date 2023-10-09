using System.Security.Cryptography;
using System.Text;

namespace PlanePal.Services.Shared
{
    public static class HashnSaltService
    {
        private const int SALT_LENGTH = 24;

        public static string HashWithSalt(string password, string salt)
        {
            byte[] passwordAsBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
            List<byte> passwordWithSaltBytes = new List<byte>();
            passwordWithSaltBytes.AddRange(passwordAsBytes);
            passwordWithSaltBytes.AddRange(saltBytes);
            byte[] digestBytes = SHA512.Create().ComputeHash(passwordWithSaltBytes.ToArray());
            return Convert.ToBase64String(digestBytes);
        }

        public static string GenerateSalt()
        {
            var random = RandomNumberGenerator.Create();
            byte[] salt = new byte[SALT_LENGTH];
            random.GetNonZeroBytes(salt);
            return Convert.ToBase64String(salt);
        }
    }
}