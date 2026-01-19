using GFATeamManager.Application.Services;
using GFATeamManager.Application.DTOS.StaffMember;
using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Interfaces.Repositories;
using Moq;

namespace GFATeamManager.Application.Tests.Services;

public class StaffMemberServiceTests
{
    private readonly Mock<IStaffMemberRepository> _staffMemberRepositoryMock;
    private readonly Mock<ISectorRepository> _sectorRepositoryMock;
    private readonly StaffMemberService _sut;

    public StaffMemberServiceTests()
    {
        _staffMemberRepositoryMock = new Mock<IStaffMemberRepository>();
        _sectorRepositoryMock = new Mock<ISectorRepository>();
        _sut = new StaffMemberService(_staffMemberRepositoryMock.Object, _sectorRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateStaffMemberSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var sectorId = Guid.NewGuid();
        var request = new CreateStaffMemberRequest
        {
            SectorId = sectorId,
            FullName = "João Silva",
            Email = "joao@test.com",
            Phone = "123456789",
            Specialty = "Fisioterapeuta"
        };

        var sector = new Sector { Id = sectorId, Name = "Fisioterapia", DepartmentId = Guid.NewGuid(), Department = new Department { Name = "Médico" }, StaffMembers = new List<StaffMember>() };

        _sectorRepositoryMock.Setup(r => r.ExistsAsync(sectorId)).ReturnsAsync(true);
        _staffMemberRepositoryMock.Setup(r => r.EmailExistsAsync(request.Email, null)).ReturnsAsync(false);
        _staffMemberRepositoryMock.Setup(r => r.AddAsync(It.IsAny<StaffMember>())).ReturnsAsync((StaffMember sm) => sm);

        // Act
        var result = await _sut.CreateAsync(userId, request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("João Silva", result.Data!.FullName);
        Assert.Equal(sectorId, result.Data.SectorId);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnFailure_WhenSectorDoesNotExist()
    {
        // Arrange
        var request = new CreateStaffMemberRequest { SectorId = Guid.NewGuid(), FullName = "Test", Phone = "123" };
        
        _sectorRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);

        // Act
        var result = await _sut.CreateAsync(Guid.NewGuid(), request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Setor não encontrado", result.Errors);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnFailure_WhenEmailAlreadyExists()
    {
        // Arrange
        var request = new CreateStaffMemberRequest { SectorId = Guid.NewGuid(), FullName = "Test", Email = "test@test.com", Phone = "123" };
        var sector = new Sector { Id = request.SectorId, Name = "Test", DepartmentId = Guid.NewGuid(), Department = new Department { Name = "Test" }, StaffMembers = new List<StaffMember>() };
        
        _sectorRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
        _staffMemberRepositoryMock.Setup(r => r.EmailExistsAsync(request.Email, null)).ReturnsAsync(true);

        // Act
        var result = await _sut.CreateAsync(Guid.NewGuid(), request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Email já cadastrado", result.Errors);
    }

    [Fact]
    public async Task CreateAsync_ShouldAllowNullEmail()
    {
        // Arrange
        var request = new CreateStaffMemberRequest { SectorId = Guid.NewGuid(), FullName = "Test", Email = null, Phone = "123" };
        var sector = new Sector { Id = request.SectorId, Name = "Test", DepartmentId = Guid.NewGuid(), Department = new Department { Name = "Test" }, StaffMembers = new List<StaffMember>() };
        
        _sectorRepositoryMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
        _staffMemberRepositoryMock.Setup(r => r.AddAsync(It.IsAny<StaffMember>())).ReturnsAsync((StaffMember sm) => sm);

        // Act
        var result = await _sut.CreateAsync(Guid.NewGuid(), request);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnStaffMemberWithSectorName()
    {
        // Arrange
        var staffId = Guid.NewGuid();
        var staff = new StaffMember 
        { 
            Id = staffId, 
            FullName = "João Silva", 
            Phone = "123",
            SectorId = Guid.NewGuid(),
            Sector = new Sector { Name = "Fisioterapia", DepartmentId = Guid.NewGuid(), Department = new Department { Name = "Médico" } }
        };

        _staffMemberRepositoryMock.Setup(r => r.GetByIdAsync(staffId)).ReturnsAsync(staff);

        // Act
        var result = await _sut.GetByIdAsync(staffId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Fisioterapia", result.Data!.SectorName);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNotFound_WhenStaffMemberDoesNotExist()
    {
        // Arrange
        _staffMemberRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((StaffMember?)null);

        // Act
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Profissional não encontrado", result.Errors);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllStaffMembers()
    {
        // Arrange
        var staffList = new List<StaffMember>
        {
            new StaffMember { Id = Guid.NewGuid(), FullName = "João", Phone = "123", SectorId = Guid.NewGuid(), Sector = new Sector { Name = "Fisio", DepartmentId = Guid.NewGuid(), Department = new Department { Name = "M" } } },
            new StaffMember { Id = Guid.NewGuid(), FullName = "Maria", Phone = "456", SectorId = Guid.NewGuid(), Sector = new Sector { Name = "Nutri", DepartmentId = Guid.NewGuid(), Department = new Department { Name = "M" } } }
        };

        _staffMemberRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(staffList);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data!.Count());
    }

    [Fact]
    public async Task GetBySectorIdAsync_ShouldReturnStaffMembersOfSector()
    {
        // Arrange
        var sectorId = Guid.NewGuid();
        var staffList = new List<StaffMember>
        {
            new StaffMember { Id = Guid.NewGuid(), FullName = "João", Phone = "123", SectorId = sectorId, Sector = new Sector { Name = "Fisio", DepartmentId = Guid.NewGuid(), Department = new Department { Name = "M" } } }
        };

        _staffMemberRepositoryMock.Setup(r => r.GetBySectorIdAsync(sectorId)).ReturnsAsync(staffList);

        // Act
        var result = await _sut.GetBySectorIdAsync(sectorId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Data!);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateStaffMemberSuccessfully()
    {
        // Arrange
        var staffId = Guid.NewGuid();
        var existing = new StaffMember { Id = staffId, FullName = "Old", Email = "old@test.com", Phone = "111", SectorId = Guid.NewGuid(), Sector = new Sector { Name = "F", DepartmentId = Guid.NewGuid(), Department = new Department { Name = "M" } } };
        var request = new UpdateStaffMemberRequest { FullName = "New", Email = "new@test.com", Phone = "222" };

        _staffMemberRepositoryMock.Setup(r => r.GetByIdAsync(staffId)).ReturnsAsync(existing);
        _staffMemberRepositoryMock.Setup(r => r.EmailExistsAsync(request.Email, staffId)).ReturnsAsync(false);
        _staffMemberRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<StaffMember>())).ReturnsAsync((StaffMember sm) => sm);

        // Act
        var result = await _sut.UpdateAsync(Guid.NewGuid(), staffId, request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("New", result.Data!.FullName);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFailure_WhenEmailAlreadyExistsForOtherStaff()
    {
        // Arrange
        var staffId = Guid.NewGuid();
        var existing = new StaffMember { Id = staffId, FullName = "Test", Email = "old@test.com", Phone = "123", SectorId = Guid.NewGuid(), Sector = new Sector { Name = "F", DepartmentId = Guid.NewGuid(), Department = new Department { Name = "M" } } };
        var request = new UpdateStaffMemberRequest { FullName = "Test", Email = "used@test.com", Phone = "123" };

        _staffMemberRepositoryMock.Setup(r => r.GetByIdAsync(staffId)).ReturnsAsync(existing);
        _staffMemberRepositoryMock.Setup(r => r.EmailExistsAsync(request.Email, staffId)).ReturnsAsync(true);

        // Act
        var result = await _sut.UpdateAsync(Guid.NewGuid(), staffId, request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Email já cadastrado", result.Errors);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteStaffMemberSuccessfully()
    {
        // Arrange
        var staffId = Guid.NewGuid();
        var staff = new StaffMember { Id = staffId, FullName = "Test", Phone = "123", SectorId = Guid.NewGuid(), Sector = new Sector { Name = "F", DepartmentId = Guid.NewGuid(), Department = new Department { Name = "M" } } };

        _staffMemberRepositoryMock.Setup(r => r.GetByIdAsync(staffId)).ReturnsAsync(staff);
        _staffMemberRepositoryMock.Setup(r => r.DeleteAsync(staffId)).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteAsync(Guid.NewGuid(), staffId);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNotFound_WhenStaffMemberDoesNotExist()
    {
        // Arrange
        _staffMemberRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((StaffMember?)null);

        // Act
        var result = await _sut.DeleteAsync(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Profissional não encontrado", result.Errors);
    }
}
