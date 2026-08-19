using System.Security.Cryptography;

namespace ToDoList.Security
{
    public static class PasswordHasher
    {
        public static (string hash, string salt) Hash(string password)
        {
            byte[] saltBytes = RandomNumberGenerator.GetBytes(16);
            byte[] hashBytes = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, 100_000, HashAlgorithmName.SHA256, 32);

            return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
        }

        public static bool Verify(string password, string storedHash, string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);
            byte[] hashBytes = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, 100_000, HashAlgorithmName.SHA256, 32);

            return Convert.ToBase64String(hashBytes) == storedHash;
        }
    }
}
