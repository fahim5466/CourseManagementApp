namespace Application.Interfaces
{
    public interface IHttpHelper
    {
        public string GetHostPathPrefix();
        public Guid GetCurrentUserId();
        public void SetAccessTokenCookie(string jwtToken);
        public void SetRefreshTokenCookie(string refreshToken);
    }
}
