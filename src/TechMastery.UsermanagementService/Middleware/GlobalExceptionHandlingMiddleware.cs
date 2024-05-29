using System;
namespace TechMastery.UsermanagementService
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                // Modify the response if needed
            }
        }
    }

    // Extension method
    public static class GlobalExceptionHandlingMiddlewareExtension
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(
            this IApplicationBuilder app) => app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }
}

