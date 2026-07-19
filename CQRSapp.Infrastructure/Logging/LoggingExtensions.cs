using Serilog;
using Serilog.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace CQRSapp.Infrastructure.Logging;

public static class LoggingExtensions
{
    public static WebApplicationBuilder AddStructuredLogging(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProperty("Application", "CQRSapp.API")
            .WriteTo.Console()
            .WriteTo.File(
                "logs/cqrsapp-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30)
            .CreateLogger();

        builder.Host.UseSerilog();
        return builder;
    }

    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            var correlationId = context.TraceIdentifier;
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
}