using GFATeamManager.Application.Services;
using GFATeamManager.Application.DTOS.Sector;
using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Interfaces.Repositories;
using Moq;

namespace GFATeamManager.Application.Tests.Services;

public class SectorServiceTests
{
    private readonly Mock<ISectorRepository> _sectorRepositoryMock;
    private readonly Mock<IDepartmentRepository> _departmentRepositoryMock;
    private readonly SectorService _sut;

    public SectorServiceTests()
    {
        _sectorRepositoryMock = new Mock<ISectorRepository>();
        _departmentRepositoryMock = new Mock<IDepartmentRepository>();
        _sut = new SectorService(_sectorRepositoryMock.Object, _departmentRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateSectorSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();
        var request = new CreateSectorRequest
        {
            DepartmentId = departmentId,
            Name = "Fisioterapia",
            Description = "Setor de fisioterapia"
        };

        var department = new Department { Id = departmentId, Name = "Médico & Performance" };

        _departmentRepositoryMock
            .Setup(r => r.ExistsAsync(departmentId))
            .ReturnsAsync(true);

        _sectorRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Sector>()))
            .ReturnsAsync((Sector s) => s);

        // Act
        var result = await _sut.CreateAsync(userId, request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("Fisioterapia", result.Data!.Name);
        Assert.Equal(departmentId, result.Data.DepartmentId);
        
        _sectorRepositoryMock.Verify(
            r => r.AddAsync(It.Is<Sector>(s => s.Name == request.Name && s.DepartmentId == departmentId)),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnFailure_WhenDepartmentDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();
        var request = new CreateSectorRequest
        {
            DepartmentId = departmentId,
            Name = "Fisioterapia",
            Description = "Test"
        };

        _departmentRepositoryMock
            .Setup(r => r.ExistsAsync(departmentId))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.CreateAsync(userId, request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Departamento não encontrado", result.Errors);
        _sectorRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Sector>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnSectorWithStaffMembers()
    {
        // Arrange
        var sectorId = Guid.NewGuid();
        var sector = new Sector
        {
            Id = sectorId,
            DepartmentId = Guid.NewGuid(),
            Name = "Fisioterapia",
            Description = "Test",
            Department = new Department { Id = Guid.NewGuid(), Name = "Médico" },
            StaffMembers = new List<StaffMember>
            {
                new StaffMember { Id = Guid.NewGuid(), FullName = "João Silva", Phone = "123456789" },
                new StaffMember { Id = Guid.NewGuid(), FullName = "Maria Santos", Phone = "987654321" }
            }
        };

        _sectorRepositoryMock
            .Setup(r => r.GetWithStaffAsync(sectorId))
            .ReturnsAsync(sector);

        // Act
        var result = await _sut.GetByIdAsync(sectorId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(sector.Id, result.Data!.Id);
        Assert.Equal(2, result.Data.StaffMembers.Count);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNotFound_WhenSectorDoesNotExist()
    {
        // Arrange
        var sectorId = Guid.NewGuid();
        
        _sectorRepositoryMock
            .Setup(r => r.GetWithStaffAsync(sectorId))
            .ReturnsAsync((Sector?)null);

        // Act
        var result = await _sut.GetByIdAsync(sectorId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Setor não encontrado", result.Errors);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllSectorsWithStaffMembers()
    {
        // Arrange
        var sectors = new List<Sector>
        {
            new Sector 
            { 
                Id = Guid.NewGuid(), 
                Name = "Fisioterapia",
                DepartmentId = Guid.NewGuid(),
                Department = new Department { Name = "Médico" },
                StaffMembers = new List<StaffMember> 
                { 
                    new StaffMember { FullName = "João", Phone = "123" } 
                }
            },
            new Sector 
            { 
                Id = Guid.NewGuid(), 
                Name = "Nutrição",
                DepartmentId = Guid.NewGuid(),
                Department = new Department { Name = "Médico" },
                StaffMembers = new List<StaffMember>()
            }
        };

        _sectorRepositoryMock
            .Setup(r => r.GetAllWithStaffAsync())
            .ReturnsAsync(sectors);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data!.Count());
    }

    [Fact]
    public async Task GetByDepartmentIdAsync_ShouldReturnSectorsOfDepartment()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var sectors = new List<Sector>
        {
            new Sector 
            { 
                Id = Guid.NewGuid(), 
                DepartmentId = departmentId, 
                Name = "Fisioterapia",
                Department = new Department { Name = "Médico" },
                StaffMembers = new List<StaffMember>() 
            }
        };

        _sectorRepositoryMock
            .Setup(r => r.GetByDepartmentIdAsync(departmentId))
            .ReturnsAsync(sectors);

        // Act
        var result = await _sut.GetByDepartmentIdAsync(departmentId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data!);
        Assert.All(result.Data, s => Assert.Equal(departmentId, s.DepartmentId));
    }

    [Fact]
    public async Task GetByDepartmentIdAsync_ShouldReturnEmptyList_WhenNoDepartmentId()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        
        _sectorRepositoryMock
            .Setup(r => r.GetByDepartmentIdAsync(departmentId))
            .ReturnsAsync(new List<Sector>());

        // Act
        var result = await _sut.GetByDepartmentIdAsync(departmentId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data!);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateSectorSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var sectorId = Guid.NewGuid();
        var existingSector = new Sector
        {
            Id = sectorId,
            DepartmentId = Guid.NewGuid(),
            Name = "Old Name",
            Description = "Old Description",
            Department = new Department { Name = "Médico" },
            StaffMembers = new List<StaffMember>()
        };

        var request = new UpdateSectorRequest
        {
            Name = "New Name",
            Description = "New Description"
        };

        _sectorRepositoryMock
            .Setup(r => r.GetByIdAsync(sectorId))
            .ReturnsAsync(existingSector);

        _sectorRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Sector>()))
            .ReturnsAsync(existingSector);

        // Act
        var result = await _sut.UpdateAsync(userId, sectorId, request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("New Name", result.Data!.Name);
        Assert.Equal("New Description", result.Data.Description);
        
        _sectorRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Sector>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNotFound_WhenSectorDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var sectorId = Guid.NewGuid();
        var request = new UpdateSectorRequest
        {
            Name = "New Name",
            Description = "New Description"
        };

        _sectorRepositoryMock
            .Setup(r => r.GetByIdAsync(sectorId))
            .ReturnsAsync((Sector?)null);

        // Act
        var result = await _sut.UpdateAsync(userId, sectorId, request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Setor não encontrado", result.Errors);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteSectorSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var sectorId = Guid.NewGuid();
        var sector = new Sector
        {
            Id = sectorId,
            DepartmentId = Guid.NewGuid(),
            Name = "Test Sector",
            Department = new Department { Name = "Test" },
            StaffMembers = new List<StaffMember>() // Empty staff
        };

        _sectorRepositoryMock
            .Setup(r => r.GetWithStaffAsync(sectorId))
            .ReturnsAsync(sector);

        _sectorRepositoryMock
            .Setup(r => r.DeleteAsync(sectorId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteAsync(userId, sectorId);

        // Assert
        Assert.True(result.IsSuccess);
        _sectorRepositoryMock.Verify(r => r.DeleteAsync(sectorId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFailure_WhenSectorHasStaffMembers()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var sectorId = Guid.NewGuid();
        var sector = new Sector
        {
            Id = sectorId,
            DepartmentId = Guid.NewGuid(),
            Name = "Test Sector",
            Department = new Department { Name = "Test" },
            StaffMembers = new List<StaffMember>
            {
                new StaffMember { Id = Guid.NewGuid(), FullName = "João Silva", Phone = "123" }
            }
        };

        _sectorRepositoryMock
            .Setup(r => r.GetWithStaffAsync(sectorId))
            .ReturnsAsync(sector);

        // Act
        var result = await _sut.DeleteAsync(userId, sectorId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Não é possível deletar setor com profissionais ativos", result.Errors);
        _sectorRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNotFound_WhenSectorDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var sectorId = Guid.NewGuid();

        _sectorRepositoryMock
            .Setup(r => r.GetWithStaffAsync(sectorId))
            .ReturnsAsync((Sector?)null);

        // Act
        var result = await _sut.DeleteAsync(userId, sectorId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Setor não encontrado", result.Errors);
    }
}
