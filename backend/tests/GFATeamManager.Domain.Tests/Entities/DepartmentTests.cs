using GFATeamManager.Domain.Entities;

namespace GFATeamManager.Domain.Tests.Entities;

public class DepartmentTests
{
    [Fact]
    public void Department_ShouldInitialize_WithIsDeletedFalse()
    {
        // Arrange & Act
        var department = new Department
        {
            Name = "Test Department"
        };

        // Assert
        Assert.False(department.IsDeleted);
    }

    [Fact]
    public void Department_Delete_ShouldMarkIsDeletedTrue()
    {
        // Arrange
        var department = new Department
        {
            Name = "Test Department"
        };

        // Act
        department.Delete();

        // Assert
        Assert.True(department.IsDeleted);
    }

    [Fact]
    public void Department_Restore_ShouldMarkIsDeletedFalse()
    {
        // Arrange
        var department = new Department
        {
            Name = "Test Department"
        };
        department.Delete(); // First delete it

        // Act
        department.Restore();

        // Assert
        Assert.False(department.IsDeleted);
    }
}
