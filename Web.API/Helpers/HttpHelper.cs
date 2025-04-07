using Application.Interfaces;
using System.Security.Claims;

namespace Web.API.Helpers
{
    public class HttpHelper(IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : IHttpHelper
    {
        public const string ACCESS_TOKEN_COOKIE_KEY = "AccessToken";
        public const string REFRESH_TOKEN_COOKIE_KEY = "RefreshToken";

        public string GetHostPathPrefix()
        {
            HttpContext? context = httpContextAccessor.HttpContext;
            if(context == null)
            {
                return string.Empty;
            }

            string host = context.Request.Host.Value!;
            string scheme = context.Request.Scheme;
            return $"{scheme}://{host}";
        }

        public Guid GetCurrentUserId()
        {
            HttpContext? context = httpContextAccessor.HttpContext;
            if (context == null)
            {
                return Guid.Empty;
            }

            string userId = context.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            Guid userGuid = Guid.Empty;
            Guid.TryParse(userId, out userGuid);
            return userGuid;
        }

        public void SetAccessTokenCookie(string jwtToken)
        {
            int jwtTokenExpiryInMinutes = Int32.Parse(configuration["Jwt:ExpirationInMinutes"]!);

            httpContextAccessor.HttpContext?.Response.Cookies.Append(ACCESS_TOKEN_COOKIE_KEY, jwtToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(jwtTokenExpiryInMinutes)
            });
        }

        public void SetRefreshTokenCookie(string refreshToken)
        {
            int refreshTokenExpiryInMinutes = Int32.Parse(configuration["RefTok:ExpirationInMinutes"]!);

            httpContextAccessor.HttpContext?.Response.Cookies.Append(REFRESH_TOKEN_COOKIE_KEY, refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(refreshTokenExpiryInMinutes),
                Path = "/api/refresh-token"
            });
        }
    }
}
