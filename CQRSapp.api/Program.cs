using CQRSapp.api;
using CQRSapp.Infrastructure.Authentication;
using CQRSapp.Infrastructure.Logging;
using CQRSapp.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// STEP 1: CONFIGURE STRUCTURED LOGGING (Serilog)
// ============================================================================
builder.AddStructuredLogging();

// ============================================================================
// STEP 2: CONFIGURE JWT SETTINGS AND AUTHENTICATION
// ============================================================================

// Load JWT Configuration from appsettings.json
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
var jwtSettings = new JwtSettings
{
    Secret = jwtSettingsSection["Secret"] ?? throw new InvalidOperationException("JWT Secret is required"),
    Issuer = jwtSettingsSection["Issuer"] ?? "CQRSapp.API",
    Audience = jwtSettingsSection["Audience"] ?? "CQRSapp.Users",
    AccessTokenExpiryMinutes = int.TryParse(jwtSettingsSection["AccessTokenExpiryMinutes"], out var accessExpiry) ? accessExpiry : 15,
    RefreshTokenExpiryDays = int.TryParse(jwtSettingsSection["RefreshTokenExpiryDays"], out var refreshExpiry) ? refreshExpiry : 7
};

builder.Services.AddSingleton(jwtSettings);

// Register JWT Token Service
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        ClockSkew = TimeSpan.Zero
    };
});

// ============================================================================
// STEP 3: CONFIGURE AUTHORIZATION
// ============================================================================
builder.Services.AddAuthorization(options =>
{
    // Define custom policies if needed
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("UserOrAdmin", policy =>
        policy.RequireRole("User", "Admin"));
});

// ============================================================================
// STEP 4: CONFIGURE SWAGGER/OPENAPI WITH JWT SUPPORT
// ============================================================================
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CQRSapp API",
        Version = "v1",
        Description = "Enterprise CQRS Application with JWT Authentication",
        Contact = new OpenApiContact
        {
            Name = "Development Team",
            Email = "dev@cqrsapp.com"
        }
    });

    // Define JWT Bearer Security Scheme
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.\r\n\r\nEnter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIs...\""
    });

    // Apply Security Requirement Globally
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ============================================================================
// STEP 5: CONFIGURE CORS
// ============================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200", "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ============================================================================
// STEP 6: CONFIGURE SERVICES
// ============================================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Register API Services via Extension Method
builder.Services.AddApiDI(builder.Configuration);

// ============================================================================
// STEP 7: CONFIGURE EXCEPTION HANDLING EXTENSION
// ============================================================================
builder.Services.AddExceptionHandling();

// ============================================================================
// BUILD THE APPLICATION
// ============================================================================
var app = builder.Build();

// ============================================================================
// STEP 8: CONFIGURE HTTP REQUEST PIPELINE
// ============================================================================

// Development Environment Configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CQRSapp API v1");
        options.RoutePrefix = string.Empty; // Serve Swagger at root
    });
}

// Global Exception Handling Middleware (MUST be first)
app.UseGlobalExceptionHandler();

// HTTPS Redirection
app.UseHttpsRedirection();

// Correlation ID Middleware for request tracing
app.UseCorrelationId();

// Request/Response Logging
app.UseRequestResponseLogging();

// CORS Middleware
app.UseCors("AllowFrontend");

// Authentication and Authorization (Order is critical!)
app.UseAuthentication();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

// ============================================================================
// START APPLICATION
// ============================================================================
app.Run();
