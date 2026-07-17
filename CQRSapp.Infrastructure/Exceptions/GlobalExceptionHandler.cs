using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using System.Diagnostics;

namespace CQRSapp.Infrastructure.Exceptions;

/// <summary>
/// Global exception handling middleware for capturing and standardizing all exceptions
/// </summary>
public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.TraceIdentifier;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            stopwatch.Stop();
            _logger.LogError(
                exception,
                "Unhandled exception occurred. CorrelationId: {CorrelationId}, Elapsed: {ElapsedMs}ms",
                correlationId,
                stopwatch.ElapsedMilliseconds);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new ExceptionDetails
            {
                CorrelationId = correlationId,
                StatusCode = context.Response.StatusCode,
                Message = "An unexpected error occurred",
                Details = exception.Message,
                Path = context.Request.Path,
                Timestamp = DateTime.UtcNow
            };

#if DEBUG
            response.Details = exception.StackTrace;
#endif

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}

/// <summary>
/// Extension methods for registering global exception handler
/// </summary>
public static class ExceptionHandlerExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandler>();
    }
}
