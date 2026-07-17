namespace CQRSapp.Infrastructure.Authentication;

/// <summary>
/// Request model for refreshing tokens
/// </summary>
public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}