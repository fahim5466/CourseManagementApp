using Application.Interfaces;
using System.Security.Claims;

namespace Web.API.Helpers
{
    public class HttpHelper(IHttpContextAccessor httpContextAccessor) : IHttpHelper
    {
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
    }
}
