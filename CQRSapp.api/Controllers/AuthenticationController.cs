using CQRSapp.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CQRSapp.api.Controllers;

/// <summary>
/// Authentication controller for handling JWT token generation, refresh, and revocation
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IJwtTokenService _tokenService;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(IJwtTokenService tokenService, ILogger<AuthenticationController> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user and returns JWT access and refresh tokens
    /// </summary>
    /// <param name="request">Login credentials (username/email and password)</param>
    /// <returns>TokenResponse containing access token and refresh token</returns>
    /// <response code="200">Successfully authenticated and tokens generated</response>
    /// <response code="401">Authentication failed - invalid credentials</response>
    /// <response code="400">Bad request - validation failed</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogWarning("Login attempt with missing credentials");
                return BadRequest(new { message = "Username and password are required" });
            }

            // TODO: Implement actual user authentication against your user store
            // This is a placeholder that demonstrates token generation
            // Replace with actual user validation logic (e.g., database lookup, password verification)
            
            // For now, validate against dummy credentials (remove in production)
            if (!ValidateCredentials(request.Username, request.Password))
            {
                _logger.LogWarning("Failed login attempt for username: {Username}", request.Username);
                return Unauthorized(new { message = "Invalid username or password" });
            }

            // Get user details (in production, retrieve from database)
            var userId = Guid.NewGuid(); // Replace with actual user ID from database
            var email = request.Username; // Replace with actual email from database
            var username = request.Username;

            // Generate tokens
            var tokenResponse = _tokenService.GenerateTokens(userId, email, username);

            _logger.LogInformation("User {Username} successfully logged in", request.Username);

            return Ok(tokenResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {Username}", request.Username);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred during authentication" });
        }
    }

    /// <summary>
    /// Refreshes an expired access token using a valid refresh token
    /// </summary>
    /// <param name="request">RefreshTokenRequest containing the refresh token</param>
    /// <returns>New TokenResponse with fresh access and refresh tokens</returns>
    /// <response code="200">Successfully refreshed tokens</response>
    /// <response code="401">Refresh token is invalid or expired</response>
    /// <response code="400">Bad request - validation failed</response>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<TokenResponse> Refresh([FromBody] RefreshTokenRequest request)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                _logger.LogWarning("Refresh token request with missing token");
                return BadRequest(new { message = "Refresh token is required" });
            }

            // Refresh tokens
            var tokenResponse = _tokenService.RefreshTokens(request.RefreshToken);

            _logger.LogInformation("Tokens successfully refreshed");

            return Ok(tokenResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Refresh token validation failed");
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred during token refresh" });
        }
    }

    /// <summary>
    /// Verifies that the user is authenticated by validating their token
    /// </summary>
    /// <returns>User information if authentication is valid</returns>
    /// <response code="200">User is authenticated</response>
    /// <response code="401">User is not authenticated</response>
    [HttpGet("verify")]
    [Authorize]
    [ProducesResponseType(typeof(VerifyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<VerifyResponse> Verify()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            _logger.LogInformation("Token verification successful for user {UserId}", userId);

            return Ok(new VerifyResponse
            {
                IsValid = true,
                UserId = userId,
                Username = username ?? string.Empty,
                Email = email ?? string.Empty,
                IssuedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token verification");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred during token verification" });
        }
    }

    /// <summary>
    /// Placeholder logout endpoint - in production, implement token blacklisting
    /// </summary>
    /// <returns>Logout confirmation</returns>
    /// <response code="200">Successfully logged out</response>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult Logout()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // TODO: Implement token blacklisting or invalidation logic here
            // For now, simply acknowledge the logout request

            _logger.LogInformation("User {UserId} logged out", userId);

            return Ok(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred during logout" });
        }
    }

    /// <summary>
    /// Validates user credentials - PLACEHOLDER for demonstration
    /// Replace with actual authentication logic (e.g., password hashing, database validation)
    /// </summary>
    private static bool ValidateCredentials(string username, string password)
    {
        // TODO: Replace with actual credential validation against your user store
        // This is a placeholder - do NOT use in production
        
        // Example dummy validation
        return !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) && password.Length >= 6;
    }
}

/// <summary>
/// Request model for user login
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// User's username or email address
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// User's password
    /// </summary>
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Response model for token verification
/// </summary>
public class VerifyResponse
{
    /// <summary>
    /// Indicates if the token is valid
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Authenticated user's ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Authenticated user's username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Authenticated user's email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Token issuance timestamp
    /// </summary>
    public DateTime IssuedAt { get; set; }
}
