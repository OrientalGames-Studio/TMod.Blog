namespace TMod.Blog.Api.Extensions
{
    internal static class HttpContextExtensions
    {
        internal static string GetClientIp(this HttpContext context)
        {
            if ( context.Request.Headers.ContainsKey("X-Forwarded-For") )
            {
                return context.Request.Headers["X-Forwarded-For"].ToString();
            }
            return context.Connection.RemoteIpAddress?.ToString() ?? "UNKNOW IP";
        }

        internal static string GetClientIp(this IHttpContextAccessor httpContextAccessor)
        {
            var context = httpContextAccessor.HttpContext;
            if ( context == null )
            {
                return "UNKNOW IP";
            }
            if ( context.Request.Headers.ContainsKey("X-Forwarded-For") )
            {
                return context.Request.Headers["X-Forwarded-For"].ToString();
            }
            return context.Connection.RemoteIpAddress?.ToString() ?? "UNKNOW IP";
        }
    }
}
