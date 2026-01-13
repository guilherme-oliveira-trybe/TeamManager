using FluentValidation;
using GFATeamManager.Api.Endpoints;
using GFATeamManager.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using GFATeamManager.Api.Middlewares;
using GFATeamManager.Infrastructure.Data.Seed;
using GFATeamManager.Application.Validators.Auth;
using GFATeamManager.Infrastructure.Data.Context;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();
builder.Services.AddCorsPolicy();
builder.Services.AddRateLimiting();
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddJwtAuthentication();
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

app.UseGlobalExceptionHandler();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        if (context.Database.IsRelational())
        {
            context.Database.Migrate();
        }
        
        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred running migrations or seed.");
        throw;
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("DynamicOrigins");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapPreRegistrationEndpoints();
app.MapUserEndpoints();
app.MapAuthEndpoints();
app.MapActivityEndpoints();

app.MapGet("/", () => new
{
    message = "GFA Team Manager API is running",
    version = "1.0.0",
    endpoints = new[]
    {
        "/swagger - API Documentation",
        "/api/auth",
        "/api/users",
        "/api/pre-registration"
    }
})
.WithName("Root");

app.Run();

public partial class Program { }