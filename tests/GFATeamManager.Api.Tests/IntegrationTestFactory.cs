using GFATeamManager.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GFATeamManager.Api.Tests;

public class IntegrationTestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("JWT_SECRET_KEY", "TestSecretKeyForIntegrationTesting123456");
        Environment.SetEnvironmentVariable("JWT_ISSUER", "GFATeamManagerTest");
        Environment.SetEnvironmentVariable("JWT_AUDIENCE", "GFATeamManagerTest");
        Environment.SetEnvironmentVariable("JWT_EXPIRATION_HOURS", "1");
        // Required for SeedData but we're skipping seed logic by manually seeding
        Environment.SetEnvironmentVariable("ROOT_EMAIL", "test@test.com");
        Environment.SetEnvironmentVariable("ROOT_PASSWORD", "test");
        Environment.SetEnvironmentVariable("ROOT_NAME", "Test");
        Environment.SetEnvironmentVariable("ROOT_CPF", "00000000000");
        Environment.SetEnvironmentVariable("ROOT_PHONE", "000000000");

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });
        });
    }
}
