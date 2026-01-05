using GFATeamManager.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace GFATeamManager.Api.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var dbHost = Environment.GetEnvironmentVariable("DATABASE_HOST");
        var dbPort = Environment.GetEnvironmentVariable("DATABASE_PORT");
        var dbName = Environment.GetEnvironmentVariable("DATABASE_NAME");
        var dbUser = Environment.GetEnvironmentVariable("DATABASE_USER");
        var dbPassword = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
        
        var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";
        
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        return services;
    }
}