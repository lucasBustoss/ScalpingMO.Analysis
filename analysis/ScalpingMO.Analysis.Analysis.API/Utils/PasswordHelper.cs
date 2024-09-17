using System.Security.Cryptography;

namespace ScalpingMO.Analysis.Analysis.API.Utils
{
    public class PasswordHelper
    {
        private const int SaltSize = 16; 
        private const int HashSize = 32; 
        private const int Iterations = 10000; 

        // Função para hashear a senha com PBKDF2 e SHA256
        public static string HashPassword(string password)
        {
            byte[] salt = new byte[SaltSize];
            RandomNumberGenerator.Fill(salt); 

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);

                byte[] hashBytes = new byte[SaltSize + HashSize];
                Array.Copy(salt, 0, hashBytes, 0, SaltSize);
                Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

                return Convert.ToBase64String(hashBytes);
            }
        }

        // Função para verificar a senha fornecida
        public static bool VerifyPassword(string password, string storedHash)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);
            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);

                for (int i = 0; i < HashSize; i++)
                {
                    if (hashBytes[i + SaltSize] != hash[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
