namespace Application.Interfaces
{
    public interface ICryptoHasher
    {
        string SimpleHash(string token);
        string EnhancedHash(string token);
        bool Verify(string token, string tokenHash);
    }
}
