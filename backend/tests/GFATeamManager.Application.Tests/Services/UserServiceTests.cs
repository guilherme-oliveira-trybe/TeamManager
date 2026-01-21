using GFATeamManager.Application.Services;
using GFATeamManager.Application.DTOS.User;
using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Enums;
using GFATeamManager.Domain.Interfaces.Repositories;
using Moq;

namespace GFATeamManager.Application.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPreRegistrationRepository> _preRegistrationRepositoryMock;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _preRegistrationRepositoryMock = new Mock<IPreRegistrationRepository>();
        _sut = new UserService(_userRepositoryMock.Object, _preRegistrationRepositoryMock.Object);
    }

    [Fact]
    public async Task CompleteRegistrationAsync_ShouldSucceed_WhenDataIsValid()
    {
        // Arrange
        var preReg = new PreRegistration 
        { 
            Id = Guid.NewGuid(), 
            Cpf = "12345678900", 
            ActivationCode = "ABC123", 
            Profile = ProfileType.Athlete, 
            IsUsed = false,
            ExpirationDate = DateTime.UtcNow.AddDays(7)
        };
        var request = new CompleteRegistrationRequest
        {
            Cpf = "123.456.789-00",
            ActivationCode = "ABC123",
            FullName = "Test User",
            Email = "test@test.com",
            Password = "Test@123",
            ConfirmPassword = "Test@123",
            BirthDate = DateTime.UtcNow.AddYears(-20),
            Weight = 75.5m,
            Height = 180,
            Phone = "11987654321",
            EmergencyContactName = "Emergency Contact",
            EmergencyContactPhone = "11912345678"
        };

        _preRegistrationRepositoryMock.Setup(r => r.GetValidPreRegistrationAsync("12345678900", "ABC123")).ReturnsAsync(preReg);
        _userRepositoryMock.Setup(r => r.EmailExistsAsync(request.Email)).ReturnsAsync(false);
        _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);
        _preRegistrationRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<PreRegistration>())).ReturnsAsync(preReg);

        // Act
        var result = await _sut.CompleteRegistrationAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task CompleteRegistrationAsync_ShouldFail_WhenPreRegistrationIsInvalid()
    {
        // Arrange
        var request = new CompleteRegistrationRequest { Cpf = "12345678900", ActivationCode = "INVALID" };
        _preRegistrationRepositoryMock.Setup(r => r.GetValidPreRegistrationAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((PreRegistration?)null);

        // Act
        var result = await _sut.CompleteRegistrationAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Código de ativação inválido ou expirado", result.Errors);
    }

    [Fact]
    public async Task CompleteRegistrationAsync_ShouldFail_WhenCodeAlreadyUsed()
    {
        // Arrange
        var preReg = new PreRegistration { IsUsed = true };
        var request = new CompleteRegistrationRequest { Cpf = "12345678900", ActivationCode = "ABC123" };
        _preRegistrationRepositoryMock.Setup(r => r.GetValidPreRegistrationAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(preReg);

        // Act
        var result = await _sut.CompleteRegistrationAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Este código já foi utilizado", result.Errors);
    }

    [Fact]
    public async Task CompleteRegistrationAsync_ShouldFail_WhenPasswordsDontMatch()
    {
        // Arrange
        var preReg = new PreRegistration { IsUsed = false };
        var request = new CompleteRegistrationRequest 
        { 
            Cpf = "12345678900", 
            ActivationCode = "ABC123",
            Password = "Pass1",
            ConfirmPassword = "Pass2"
        };
        _preRegistrationRepositoryMock.Setup(r => r.GetValidPreRegistrationAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(preReg);

        // Act
        var result = await _sut.CompleteRegistrationAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("As senhas não coincidem", result.Errors);
    }

    [Fact]
    public async Task CompleteRegistrationAsync_ShouldFail_WhenEmailAlreadyExists()
    {
        // Arrange
        var preReg = new PreRegistration { IsUsed = false };
        var request = new CompleteRegistrationRequest 
        { 
            Cpf = "12345678900", 
            ActivationCode = "ABC123",
            Email = "existing@test.com",
            Password = "Pass1",
            ConfirmPassword = "Pass1"
        };
        _preRegistrationRepositoryMock.Setup(r => r.GetValidPreRegistrationAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(preReg);
        _userRepositoryMock.Setup(r => r.EmailExistsAsync(request.Email)).ReturnsAsync(true);

        // Act
        var result = await _sut.CompleteRegistrationAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Este email já está cadastrado", result.Errors);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, FullName = "Test User", Cpf = "12345678900", Email = "test@test.com", Phone = "11987654321" };
        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _sut.GetByIdAsync(userId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Data!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        // Act
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Usuário não encontrado", result.Errors);
    }

    [Fact]
    public async Task GetByCpfAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User { Cpf = "12345678900", FullName = "Test", Email = "test@test.com", Phone = "11987654321" };
        _userRepositoryMock.Setup(r => r.GetByCpfAsync("12345678900")).ReturnsAsync(user);

        // Act
        var result = await _sut.GetByCpfAsync("123.456.789-00");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("12345678900", result.Data!.Cpf);
    }

    [Fact]
    public async Task GetByStatusAsync_ShouldReturnUsers_WithGivenStatus()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Status = UserStatus.Active, Cpf = "111", FullName = "User1", Email = "u1@t.com", Phone = "11" },
            new User { Status = UserStatus.Active, Cpf = "222", FullName = "User2", Email = "u2@t.com", Phone = "22" }
        };
        _userRepositoryMock.Setup(r => r.GetByStatusAsync(UserStatus.Active)).ReturnsAsync(users);

        // Act
        var result = await _sut.GetByStatusAsync(UserStatus.Active);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data!.Count);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser_WhenDataIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingUser = new User { Id = userId, Email = "old@test.com", FullName = "Old Name", Cpf = "12345678900", Phone = "11987654321" };
        var request = new UpdateUserRequest { Email = "new@test.com", FullName = "New Name", BirthDate = DateTime.UtcNow.AddYears(-25), Weight = 80, Height = 175, Phone = "11999999999" };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(existingUser);
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);

        // Act
        var result = await _sut.UpdateAsync(userId, request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("New Name", result.Data!.FullName);
    }
}
