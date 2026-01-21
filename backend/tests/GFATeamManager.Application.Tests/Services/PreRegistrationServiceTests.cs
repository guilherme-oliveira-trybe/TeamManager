using GFATeamManager.Application.Services;
using GFATeamManager.Application.DTOS.PreRegistration;
using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Enums;
using GFATeamManager.Domain.Interfaces.Repositories;
using Moq;

namespace GFATeamManager.Application.Tests.Services;

public class PreRegistrationServiceTests
{
    private readonly Mock<IPreRegistrationRepository> _preRegistrationRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly PreRegistrationService _sut;

    public PreRegistrationServiceTests()
    {
        _preRegistrationRepositoryMock = new Mock<IPreRegistrationRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _sut = new PreRegistrationService(_preRegistrationRepositoryMock.Object, _userRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreatePreRegistration_WhenDataIsValid()
    {
        // Arrange
        var request = new CreatePreRegistrationRequest
        {
            Cpf = "11144477735", // Valid CPF
            Profile = ProfileType.Athlete,
            Unit = PlayerUnit.Offense,
            Position = PlayerPosition.QB
        };

        _userRepositoryMock.Setup(r => r.CpfExistsAsync(request.Cpf)).ReturnsAsync(false);
        _preRegistrationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<PreRegistration>())).ReturnsAsync((PreRegistration p) => p);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("11144477735", result.Data!.Cpf);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenCpfIsInvalid()
    {
        // Arrange
        var request = new CreatePreRegistrationRequest
        {
            Cpf = "00000000000", // Invalid CPF
            Profile = ProfileType.Athlete
        };

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("CPF inválido", result.Errors);
    }

    [Fact]
    public async Task CreateAsync_ShouldFail_WhenCpfAlreadyExists()
    {
        // Arrange
        var request = new CreatePreRegistrationRequest
        {
            Cpf = "11144477735", // Valid CPF
            Profile = ProfileType.Athlete
        };

        _userRepositoryMock.Setup(r => r.CpfExistsAsync(request.Cpf)).ReturnsAsync(true);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Já existe um usuário cadastrado com este CPF", result.Errors);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPreRegistration_WhenExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var preReg = new PreRegistration
        {
            Id = id,
            Cpf = "12345678900",
            ActivationCode = "ABC123",
            Profile = ProfileType.Athlete
        };

        _preRegistrationRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(preReg);

        // Act
        var result = await _sut.GetByIdAsync(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Data!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNotFound_WhenDoesNotExist()
    {
        // Arrange
        _preRegistrationRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((PreRegistration?)null);

        // Act
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Pré-cadastro não encontrado", result.Errors);
    }

    [Fact]
    public async Task GetByCpfAsync_ShouldReturnPreRegistrations_ForGivenCpf()
    {
        // Arrange
        var preRegs = new List<PreRegistration>
        {
            new PreRegistration { Cpf = "12345678900", ActivationCode = "ABC", Profile = ProfileType.Athlete },
            new PreRegistration { Cpf = "12345678900", ActivationCode = "DEF", Profile = ProfileType.Admin }
        };

        _preRegistrationRepositoryMock.Setup(r => r.GetUnusedByCpfAsync("12345678900")).ReturnsAsync(preRegs);

        // Act
        var result = await _sut.GetByCpfAsync("123.456.789-00");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data!.Count);
    }

    [Fact]
    public async Task GetByCpfAsync_ShouldReturnEmptyList_WhenNoPreRegistrations()
    {
        // Arrange
        _preRegistrationRepositoryMock.Setup(r => r.GetUnusedByCpfAsync(It.IsAny<string>())).ReturnsAsync(new List<PreRegistration>());

        // Act
        var result = await _sut.GetByCpfAsync("12345678900");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data!);
    }

    [Fact]
    public async Task RegenerateCodeAsync_ShouldSucceed_WhenPreRegistrationExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var preReg = new PreRegistration
        {
            Id = id,
            Cpf = "12345678900",
            Profile = ProfileType.Athlete,
            IsUsed = false
        };

        _preRegistrationRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(preReg);
        _preRegistrationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<PreRegistration>())).ReturnsAsync((PreRegistration p) => p);

        // Act
        var result = await _sut.RegenerateCodeAsync(id);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task RegenerateCodeAsync_ShouldFail_WhenPreRegistrationDoesNotExist()
    {
        // Arrange
        _preRegistrationRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((PreRegistration?)null);

        // Act
        var result = await _sut.RegenerateCodeAsync(Guid.NewGuid());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Pré-cadastro não encontrado", result.Errors);
    }

    [Fact]
    public async Task RegenerateCodeAsync_ShouldFail_WhenAlreadyUsed()
    {
        // Arrange
        var id = Guid.NewGuid();
        var preReg = new PreRegistration
        {
            Id = id,
            Cpf = "12345678900",
            IsUsed = true
        };

        _preRegistrationRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(preReg);

        // Act
        var result = await _sut.RegenerateCodeAsync(id);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Este pré-cadastro já foi utilizado", result.Errors);
    }
}
