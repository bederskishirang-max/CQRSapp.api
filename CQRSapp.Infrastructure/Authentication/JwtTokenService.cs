using CQRSapp.Infrastructure.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Logging;

namespace CQRSapp.Infrastructure.Authentication;

/// <summary>
/// JWT token generation and validation service
/// </summary>
public interface IJwtTokenService
{
    TokenResponse GenerateTokens(Guid userId, string email, string username);
    ClaimsPrincipal? ValidateToken(string token);
    TokenResponse RefreshTokens(string refreshToken);
}

/// <summary>
/// Implementation of JWT token service with refresh token support
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<JwtTokenService> _logger;
    private readonly SymmetricSecurityKey _key;

    // In-memory store for refresh tokens (replace with database in production)
    private static readonly Dictionary<string, RefreshTokenData> RefreshTokenStore = new();

    public JwtTokenService(JwtSettings jwtSettings, ILogger<JwtTokenService> logger)
    {
        _jwtSettings = jwtSettings;
        _logger = logger;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
    }

    public TokenResponse GenerateTokens(Guid userId, string email, string username)
    {
        try
        {
            var accessToken = GenerateAccessToken(userId, email, username);
            var refreshToken = GenerateRefreshToken();

            // Store refresh token (use database in production)
            RefreshTokenStore[refreshToken] = new RefreshTokenData
            {
                UserId = userId,
                ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
            };

            _logger.LogInformation(
                "Tokens generated successfully for user {UserId}",
                userId);

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = _jwtSettings.AccessTokenExpiryMinutes * 60,
                TokenType = "Bearer"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating tokens for user {UserId}", userId);
            throw;
        }
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return null;
        }
    }

    public TokenResponse RefreshTokens(string refreshToken)
    {
        if (!RefreshTokenStore.TryGetValue(refreshToken, out var tokenData))
        {
            _logger.LogWarning("Invalid refresh token attempt");
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        if (DateTime.UtcNow > tokenData.ExpiryDate)
        {
            RefreshTokenStore.Remove(refreshToken);
            _logger.LogWarning("Refresh token expired for user {UserId}", tokenData.UserId);
            throw new UnauthorizedAccessException("Refresh token expired");
        }

        // Generate new tokens
        var newAccessToken = GenerateAccessToken(tokenData.UserId, "", "");
        var newRefreshToken = GenerateRefreshToken();

        // Remove old refresh token and store new one
        RefreshTokenStore.Remove(refreshToken);
        RefreshTokenStore[newRefreshToken] = new RefreshTokenData
        {
            UserId = tokenData.UserId,
            ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
        };

        _logger.LogInformation("Tokens refreshed for user {UserId}", tokenData.UserId);

        return new TokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = _jwtSettings.AccessTokenExpiryMinutes * 60,
            TokenType = "Bearer"
        };
    }

    private string GenerateAccessToken(Guid userId, string email, string username)
    {
        var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email ?? ""),
            new(ClaimTypes.Name, username ?? ""),
            new("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    private class RefreshTokenData
    {
        public Guid UserId { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
