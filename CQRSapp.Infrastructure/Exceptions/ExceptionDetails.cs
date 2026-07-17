namespace CQRSapp.Infrastructure.Exceptions;

/// <summary>
/// Standardized exception response DTO for all API errors
/// </summary>
public class ExceptionDetails
{
    public string CorrelationId { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Path { get; set; }
    public Dictionary<string, string[]>? ValidationErrors { get; set; }

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}
