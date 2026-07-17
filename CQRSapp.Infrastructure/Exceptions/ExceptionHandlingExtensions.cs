using Microsoft.Extensions.DependencyInjection;

namespace CQRSapp.Infrastructure.Exceptions;

/// <summary>
/// Extension methods for registering exception handling services in the DI container
/// </summary>
public static class ExceptionHandlingExtensions
{
    /// <summary>
    /// Adds exception handling services to the DI container
    /// </summary>
    public static IServiceCollection AddExceptionHandling(this IServiceCollection services)
    {
        // Exception handling is configured via middleware in Program.cs
        // This extension method is provided for consistency and future extensibility
        return services;
    }
}
