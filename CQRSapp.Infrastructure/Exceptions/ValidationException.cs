namespace CQRSapp.Infrastructure.Exceptions;

/// <summary>
/// Custom exception for validation errors with detailed field-level information
/// </summary>
public class ValidationException : Exception
{
    public Dictionary<string, string[]> Errors { get; set; } = new();

    public ValidationException(string message = "One or more validation failures have occurred.") 
        : base(message)
    {
    }

    public ValidationException(Dictionary<string, string[]> errors) 
        : base("One or more validation failures have occurred.")
    {
        Errors = errors;
    }

    public ValidationException(string key, string[] messages) 
        : base("One or more validation failures have occurred.")
    {
        Errors.Add(key, messages);
    }
}
