using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CQRSapp.Infrastructure.Logging;

/// <summary>
/// Extension methods for configuring Serilog structured logging
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Configures Serilog as the logging provider with structured logging
    /// </summary>
    public static WebApplicationBuilder AddStructuredLogging(this WebApplicationBuilder builder)
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProperty("Application", "CQRSapp.API")
            .WriteTo.Console(outputTemplate: 
                "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                "logs/cqrsapp-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
                retainedFileCountLimit: 30)
            .CreateLogger();

        builder.Host.UseSerilog();
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        return builder;
    }

    /// <summary>
    /// Adds correlation ID middleware for request tracing
    /// </summary>
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            var correlationId = context.TraceIdentifier;
            
            // Add correlation ID to log context so it appears in all logs for this request
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers.Add("X-Correlation-Id", correlationId);
                    return Task.CompletedTask;
                });

                await next(context);
            }
        });
    }

    /// <summary>
    /// Adds request/response logging middleware
    /// </summary>
    public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}

/// <summary>
/// Middleware for logging incoming requests and outgoing responses
/// </summary>
public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var request = await LogRequestAsync(context);
        
        var originalBodyStream = context.Response.Body;

        using (var responseBody = new MemoryStream())
        {
            context.Response.Body = responseBody;

            try
            {
                await _next(context);

                await LogResponseAsync(context);

                await responseBody.CopyToAsync(originalBodyStream);
            }
            catch
            {
                await responseBody.CopyToAsync(originalBodyStream);
                throw;
            }
        }
    }

    private async Task<string> LogRequestAsync(HttpContext context)
    {
        context.Request.EnableBuffering();
        var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0;

        _logger.LogInformation(
            "HTTP Request: {Method} {Path} | Query: {Query} | Body: {Body}",
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString,
            string.IsNullOrEmpty(body) ? "N/A" : body[..Math.Min(500, body.Length)]);

        return body;
    }

    private async Task LogResponseAsync(HttpContext context)
    {
        _logger.LogInformation(
            "HTTP Response: {StatusCode} | Content-Type: {ContentType}",
            context.Response.StatusCode,
            context.Response.ContentType);
    }
}
