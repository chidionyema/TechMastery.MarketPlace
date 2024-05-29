namespace TechMastery.UsermanagementService
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Example header; add more as needed
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

            await _next(context);
        }
    }

    // Extension method for easy middleware registration
    public static class SecurityHeadersMiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(
            this IApplicationBuilder builder) => builder.UseMiddleware<SecurityHeadersMiddleware>();
    }

}

