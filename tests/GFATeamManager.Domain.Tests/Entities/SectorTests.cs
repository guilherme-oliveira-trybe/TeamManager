using GFATeamManager.Domain.Entities;

namespace GFATeamManager.Domain.Tests.Entities;

public class SectorTests
{
    [Fact]
    public void Sector_ShouldInitialize_WithIsDeletedFalse()
    {
        // Arrange & Act
        var sector = new Sector
        {
            Name = "Test Sector",
            DepartmentId = Guid.NewGuid()
        };

        // Assert
        Assert.False(sector.IsDeleted);
    }

    [Fact]
    public void Sector_Delete_ShouldMarkIsDeletedTrue()
    {
        // Arrange
        var sector = new Sector
        {
            Name = "Test Sector",
            DepartmentId = Guid.NewGuid()
        };

        // Act
        sector.Delete();

        // Assert
        Assert.True(sector.IsDeleted);
    }

    [Fact]
    public void Sector_Restore_ShouldMarkIsDeletedFalse()
    {
        // Arrange
        var sector = new Sector
        {
            Name = "Test Sector",
            DepartmentId = Guid.NewGuid()
        };
        sector.Delete(); // First delete it

        // Act
        sector.Restore();

        // Assert
        Assert.False(sector.IsDeleted);
    }
}
