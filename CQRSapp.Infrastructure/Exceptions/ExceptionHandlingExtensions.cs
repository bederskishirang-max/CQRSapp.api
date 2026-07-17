using CQRSapp.Infrastructure.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace CQRSapp.Infrastructure.Logging;

/// <summary>
/// Extension methods for configuring exception handling services
/// </summary>
public static class ExceptionHandlingExtensions
{
    public static IServiceCollection AddExceptionHandling(this IServiceCollection services)
    {
        // Exception handling is registered as middleware in Program.cs
        // This method is for future expansion if needed (e.g., custom exception handlers, etc.)
        return services;
    }
}
