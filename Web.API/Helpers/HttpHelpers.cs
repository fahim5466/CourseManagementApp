namespace Web.API.Helpers
{
    public static class HttpHelpers
    {
        public static string GetHostPathPrefix(HttpContext context)
        {
            string host = context.Request.Host.Value!;
            string scheme = context.Request.Scheme;
            return $"{scheme}://{host}";
        }
    }
}
