using GFATeamManager.Domain.Entities;

namespace GFATeamManager.Domain.Tests.Entities;

public class StaffMemberEntityTests
{
    [Fact]
    public void StaffMember_ShouldInitialize_WithIsDeletedFalse()
    {
        // Arrange & Act
        var staffMember = new StaffMember
        {
            FullName = "Test Staff",
            Phone = "11987654321",
            SectorId = Guid.NewGuid()
        };

        // Assert
        Assert.False(staffMember.IsDeleted);
    }

    [Fact]
    public void StaffMember_Delete_ShouldMarkIsDeletedTrue()
    {
        // Arrange
        var staffMember = new StaffMember
        {
            FullName = "Test Staff",
            Phone = "11987654321",
            SectorId = Guid.NewGuid()
        };

        // Act
        staffMember.Delete();

        // Assert
        Assert.True(staffMember.IsDeleted);
    }

    [Fact]
    public void StaffMember_Restore_ShouldMarkIsDeletedFalse()
    {
        // Arrange
        var staffMember = new StaffMember
        {
            FullName = "Test Staff",
            Phone = "11987654321",
            SectorId = Guid.NewGuid()
        };
        staffMember.Delete(); // First delete it

        // Act
        staffMember.Restore();

        // Assert
        Assert.False(staffMember.IsDeleted);
    }
}
