using System.Net;
using System.Text.Json;

namespace CustomerOnboarding.Api.Middleware
{
    public sealed class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;


        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                context.Response.ContentType = "application/json";
                var code = ex switch
                {
                    InvalidOperationException => (int)HttpStatusCode.BadRequest,
                    ArgumentException => (int)HttpStatusCode.BadRequest,
                    KeyNotFoundException => (int)HttpStatusCode.NotFound,
                    _ => (int)HttpStatusCode.InternalServerError
                };
                context.Response.StatusCode = code;
                var result = JsonSerializer.Serialize(new { error = ex.Message });
                await context.Response.WriteAsync(result);
            }
        }
    }
}
