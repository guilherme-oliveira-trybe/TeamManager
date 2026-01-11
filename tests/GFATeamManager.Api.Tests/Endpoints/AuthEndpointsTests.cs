using System.Net;
using System.Net.Http.Json;
using BCrypt.Net;
using FluentAssertions;
using GFATeamManager.Application.DTOS.Auth;
using GFATeamManager.Application.DTOS.Common;
using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Enums;
using GFATeamManager.Infrastructure.Data.Context;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GFATeamManager.Api.Tests.Endpoints;

public class AuthEndpointsTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;
    private readonly HttpClient _client;

    public AuthEndpointsTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Login_ShouldReturnSuccess_WhenCredentialsAreValid()
    {
        // Arrange
        const string password = "TestPassword123";
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            if (!dbContext.Users.Any(u => u.Email == "test@user.com"))
            {
                var preRegistration = new PreRegistration
                {
                    Cpf = "11122233344",
                    Profile = ProfileType.Admin
                };
                dbContext.PreRegistrations.Add(preRegistration);

                var user = new User
                {
                    FullName = "Test User",
                    Email = "test@user.com",
                    Cpf = "11122233344",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    Profile = ProfileType.Admin,
                    Status = UserStatus.Active,
                    BirthDate = DateTime.Now.AddYears(-20),
                    Phone = "123456789",
                    PreRegistrationId = preRegistration.Id,
                    PreRegistration = preRegistration
                };
                dbContext.Users.Add(user);
                dbContext.SaveChanges();
            }
        }

        var request = new LoginRequest
        {
            Login = "test@user.com",
            Password = password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<BaseResponse<LoginResponse>>();
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.Data!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var request = new LoginRequest
        {
            Login = "nonexistent@user.com", 
            Password = "WrongPassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
