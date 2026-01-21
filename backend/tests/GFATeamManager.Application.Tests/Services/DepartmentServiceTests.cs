using GFATeamManager.Application.Services;
using GFATeamManager.Application.DTOS.Department;
using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Interfaces.Repositories;
using Moq;

namespace GFATeamManager.Application.Tests.Services;

public class DepartmentServiceTests
{
    private readonly Mock<IDepartmentRepository> _departmentRepositoryMock;
    private readonly DepartmentService _sut;

    public DepartmentServiceTests()
    {
        _departmentRepositoryMock = new Mock<IDepartmentRepository>();
        _sut = new DepartmentService(_departmentRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateDepartmentSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateDepartmentRequest
        {
            Name = "Médico & Performance",
            Description = "Departamento responsável pela saúde"
        };

        _departmentRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Department>()))
            .ReturnsAsync((Department d) => d);

        // Act
        var result = await _sut.CreateAsync(userId, request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("Médico & Performance", result.Data!.Name);
        Assert.Equal("Departamento responsável pela saúde", result.Data.Description);
        
        _departmentRepositoryMock.Verify(
            r => r.AddAsync(It.Is<Department>(d => d.Name == request.Name && d.Description == request.Description)),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateAsync_ShouldSetCreatedAtDate()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateDepartmentRequest
        {
            Name = "Test Department",
            Description = "Test"
        };

        _departmentRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Department>()))
            .ReturnsAsync((Department d) => d);

        // Act
        var result = await _sut.CreateAsync(userId, request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(default, result.Data!.CreatedAt);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnDepartmentWithSectors()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var department = new Department
        {
            Id = departmentId,
            Name = "Médico & Performance",
            Description = "Test",
            Sectors = new List<Sector>
            {
                new Sector { Id = Guid.NewGuid(), Name = "Fisioterapia", StaffMembers = new List<StaffMember>() },
                new Sector { Id = Guid.NewGuid(), Name = "Nutrição", StaffMembers = new List<StaffMember>() }
            }
        };

        _departmentRepositoryMock
            .Setup(r => r.GetByIdAsync(departmentId))
            .ReturnsAsync(department);

        // Act
        var result = await _sut.GetByIdAsync(departmentId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(departmentId, result.Data!.Id);
        Assert.Equal(2, result.Data.Sectors.Count);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNotFound_WhenDepartmentDoesNotExist()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        
        _departmentRepositoryMock
            .Setup(r => r.GetByIdAsync(departmentId))
            .ReturnsAsync((Department?)null);

        // Act
        var result = await _sut.GetByIdAsync(departmentId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Departamento não encontrado", result.Errors);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllDepartmentsWithSectors()
    {
        // Arrange
        var departments = new List<Department>
        {
            new Department 
            { 
                Id = Guid.NewGuid(), 
                Name = "Médico & Performance",
                Sectors = new List<Sector> { new Sector { Id = Guid.NewGuid(), Name = "Fisioterapia", StaffMembers = new List<StaffMember>() } }
            },
            new Department 
            { 
                Id = Guid.NewGuid(), 
                Name = "Administrativo",
                Sectors = new List<Sector>()
            }
        };

        _departmentRepositoryMock
            .Setup(r => r.GetAllWithSectorsAsync())
            .ReturnsAsync(departments);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data!.Count());
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateDepartmentSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();
        var existingDepartment = new Department
        {
            Id = departmentId,
            Name = "Old Name",
            Description = "Old Description"
        };

        var request = new UpdateDepartmentRequest
        {
            Name = "New Name",
            Description = "New Description"
        };

        _departmentRepositoryMock
            .Setup(r => r.GetByIdAsync(departmentId))
            .ReturnsAsync(existingDepartment);

        _departmentRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Department>()))
            .ReturnsAsync(existingDepartment);

        // Act
        var result = await _sut.UpdateAsync(userId, departmentId, request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("New Name", result.Data!.Name);
        Assert.Equal("New Description", result.Data.Description);
        
        _departmentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Department>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNotFound_WhenDepartmentDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();
        var request = new UpdateDepartmentRequest
        {
            Name = "New Name",
            Description = "New Description"
        };

        _departmentRepositoryMock
            .Setup(r => r.GetByIdAsync(departmentId))
            .ReturnsAsync((Department?)null);

        // Act
        var result = await _sut.UpdateAsync(userId, departmentId, request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Departamento não encontrado", result.Errors);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteDepartmentSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();
        var department = new Department
        {
            Id = departmentId,
            Name = "Test Department",
            Sectors = new List<Sector>() // Empty sectors
        };

        _departmentRepositoryMock
            .Setup(r => r.GetByIdAsync(departmentId))
            .ReturnsAsync(department);

        _departmentRepositoryMock
            .Setup(r => r.DeleteAsync(departmentId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteAsync(userId, departmentId);

        // Assert
        Assert.True(result.IsSuccess);
        _departmentRepositoryMock.Verify(r => r.DeleteAsync(departmentId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFailure_WhenDepartmentHasSectors()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();
        var department = new Department
        {
            Id = departmentId,
            Name = "Test Department",
            Sectors = new List<Sector>
            {
                new Sector { Id = Guid.NewGuid(), Name = "Fisioterapia", StaffMembers = new List<StaffMember>() }
            }
        };

        _departmentRepositoryMock
            .Setup(r => r.GetByIdAsync(departmentId))
            .ReturnsAsync(department);

        // Act
        var result = await _sut.DeleteAsync(userId, departmentId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Não é possível deletar departamento com setores ativos", result.Errors);
        _departmentRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNotFound_WhenDepartmentDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();

        _departmentRepositoryMock
            .Setup(r => r.GetByIdAsync(departmentId))
            .ReturnsAsync((Department?)null);

        // Act
        var result = await _sut.DeleteAsync(userId, departmentId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Departamento não encontrado", result.Errors);
    }
}
