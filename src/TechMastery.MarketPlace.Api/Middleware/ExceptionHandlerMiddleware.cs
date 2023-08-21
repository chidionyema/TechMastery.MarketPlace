using System.Net;
using System.Text.Json;
using TechMastery.MarketPlace.Application.Exceptions;

namespace TechMastery.MarketPlace.Api.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An error occurred while processing the request.");

            var statusCode = GetStatusCodeForException(exception);
            var response = context.Response;

            response.ContentType = "application/json";
            response.StatusCode = (int)statusCode;

            var errorMessage = GetErrorMessageForStatusCode(statusCode, exception);

            var errorResponse = JsonSerializer.Serialize(new { error = errorMessage });

            await response.WriteAsync(errorResponse);
        }

        private HttpStatusCode GetStatusCodeForException(Exception exception)
        {
            return exception switch
            {
                ValidationException _ => HttpStatusCode.BadRequest,
                BadRequestException _ => HttpStatusCode.BadRequest,
                NotFoundException _ => HttpStatusCode.NotFound,
                _ => HttpStatusCode.InternalServerError,
            };
        }

        private string GetErrorMessageForStatusCode(HttpStatusCode statusCode, Exception exception)
        {
            return statusCode switch
            {
                HttpStatusCode.BadRequest => exception.Message,
                HttpStatusCode.NotFound => "Resource not found.",
                _ => "An unexpected error occurred.",
            };
        }
    }
}
