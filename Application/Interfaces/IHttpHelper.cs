namespace Application.Interfaces
{
    public interface IHttpHelper
    {
        public string GetHostPathPrefix();
        public Guid GetCurrentUserId();
        public void SetAccessTokenCookie(string jwtToken, bool expireCookie = false);
        public void SetRefreshTokenCookie(string refreshToken, bool expireCookie = false);
    }
}
