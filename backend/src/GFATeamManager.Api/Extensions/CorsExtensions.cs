namespace GFATeamManager.Api.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        var origins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?.Split(",") ?? ["http://localhost:3000"];

        services.AddCors(options =>
        {
            options.AddPolicy("DynamicOrigins", policy =>
            {
                policy.WithOrigins(origins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        return services;
    }
}