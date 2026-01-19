using System.Text;
using GFATeamManager.Application.Services;
using GFATeamManager.Application.Services.Interfaces;
using GFATeamManager.Application.Settings;
using GFATeamManager.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace GFATeamManager.Api.Extensions;

public static class JwtExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? throw new InvalidOperationException("JWT Secret Key not configured");

        var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? throw new InvalidOperationException("JWT ISSUER not configured");
        
        var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? throw new InvalidOperationException("JWT AUDIENCE not configured");

        var expirationHours = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRATION_HOURS") ?? "8");

        var jwtSettings = new JwtSettings
        {
            SecretKey = jwtSecretKey,
            Issuer = issuer,
            Audience = audience,
            ExpirationInHours = expirationHours
        };

        services.Configure<JwtSettings>(options =>
        {
            options.SecretKey = jwtSettings.SecretKey;
            options.Issuer = jwtSettings.Issuer;
            options.Audience = jwtSettings.Audience;
            options.ExpirationInHours = jwtSettings.ExpirationInHours;
        });

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorizationBuilder().AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

        return services;
    }
}