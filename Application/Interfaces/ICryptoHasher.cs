namespace Application.Interfaces
{
    public interface ICryptoHasher
    {
        public string SimpleHash(string token);
        public string EnhancedHash(string token);
        public bool Verify(string token, string tokenHash);
    }
}
