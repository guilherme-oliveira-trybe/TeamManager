using FluentAssertions;
using GFATeamManager.Application.DTOS.Auth;
using GFATeamManager.Application.Services;
using GFATeamManager.Application.Services.Interfaces;
using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Enums;
using GFATeamManager.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace GFATeamManager.Application.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<IPasswordResetRequestRepository> _passwordResetRequestRepositoryMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _jwtServiceMock = new Mock<IJwtService>();
        _passwordResetRequestRepositoryMock = new Mock<IPasswordResetRequestRepository>();
        
        _authService = new AuthService(
            _userRepositoryMock.Object,
            _jwtServiceMock.Object,
            _passwordResetRequestRepositoryMock.Object
        );
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var request = new LoginRequest { Login = "wrong@email.com", Password = "password" };
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Login))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Login ou senha inválidos");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenPasswordIsInvalid()
    {
        // Arrange
        var request = new LoginRequest { Login = "user@email.com", Password = "wrongpassword" };
        var user = new User
        {
            Email = request.Login,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
            Status = UserStatus.Active
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Login))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Login ou senha inválidos");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenUserIsNotActive()
    {
        // Arrange
        var request = new LoginRequest { Login = "user@email.com", Password = "password" };
        var user = new User
        {
            Email = request.Login,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Status = UserStatus.AwaitingActivation
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Login))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Usuário não está ativo. Entre em contato com o administrador.");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnSuccess_WhenCredentialsAreValidAndUserIsActive()
    {
        // Arrange
        var request = new LoginRequest { Login = "user@email.com", Password = "password" };
        var user = new User
        {
            Email = request.Login,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Status = UserStatus.Active
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Login))
            .ReturnsAsync(user);
        
        _jwtServiceMock.Setup(j => j.GenerateToken(user))
            .Returns("valid_token");

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Token.Should().Be("valid_token");
    }
}
