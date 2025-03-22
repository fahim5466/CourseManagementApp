using Application.Interfaces;

namespace Infrastructure.Security
{
    class CryptoHasher : ICryptoHasher
    {
        private const int simpleHashWorkFactor = 5;
        private const int enhancedHashWorkFactor = 13;
        
        public string SimpleHash(string token)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(token, simpleHashWorkFactor);
        }

        public string EnhancedHash(string token)
        {
            return BCrypt.Net.BCrypt.EnhancedHashPassword(token, enhancedHashWorkFactor);
        }

        public bool Verify(string token, string tokenHash)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(token, tokenHash);
        }
    }
}
