namespace CQRSapp.Infrastructure.Exceptions;

/// <summary>
/// Represents detailed exception information for API responses
/// </summary>
public class ExceptionDetails
{
    public string CorrelationId { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public Dictionary<string, string[]>? ValidationErrors { get; set; }
}
