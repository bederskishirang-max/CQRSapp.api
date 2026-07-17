namespace CQRSapp.Infrastructure.Exceptions;

/// <summary>
/// Represents validation exception with detailed error information
/// </summary>
public class ValidationException : Exception
{
    public Dictionary<string, string[]> Errors { get; set; } = new();

    public ValidationException(string message) : base(message)
    {
    }

    public ValidationException(string message, Dictionary<string, string[]> errors) 
        : base(message)
    {
        Errors = errors;
    }

    public ValidationException(Dictionary<string, string[]> errors) 
        : base("One or more validation failures have occurred.")
    {
        Errors = errors;
    }
}
