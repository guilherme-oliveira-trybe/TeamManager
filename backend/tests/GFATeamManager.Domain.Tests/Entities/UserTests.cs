using FluentAssertions;
using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Enums;
using Xunit;

namespace GFATeamManager.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void Constructor_ShouldSetDefaultStatus_ToPendingRegistration()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        user.Status.Should().Be(UserStatus.PendingRegistration);
    }

    [Fact]
    public void Activate_ShouldSetStatusToActive_AndSetActivationData()
    {
        // Arrange
        var user = new User { Status = UserStatus.AwaitingActivation };
        var adminId = Guid.NewGuid();
        var beforeActivation = DateTime.UtcNow;

        // Act
        user.Activate(adminId);

        // Assert
        user.Status.Should().Be(UserStatus.Active);
        user.ActivatedAt.Should().NotBeNull();
        user.ActivatedAt.Should().BeCloseTo(beforeActivation, TimeSpan.FromSeconds(1));
        user.ActivatedById.Should().Be(adminId);
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Reject_ShouldSetStatusToRejected_AndUpdateTimestamp()
    {
        // Arrange
        var user = new User { Status = UserStatus.AwaitingActivation };
        var beforeReject = DateTime.UtcNow;

        // Act
        user.Reject();

        // Assert
        user.Status.Should().Be(UserStatus.Rejected);
        user.UpdatedAt.Should().BeCloseTo(beforeReject, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Deactivate_ShouldSetStatusToInactive_AndUpdateTimestamp()
    {
        // Arrange
        var user = new User { Status = UserStatus.Active };
        var beforeDeactivate = DateTime.UtcNow;

        // Act
        user.Deactivate();

        // Assert
        user.Status.Should().Be(UserStatus.Inactive);
        user.UpdatedAt.Should().BeCloseTo(beforeDeactivate, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void IsActive_ShouldReturnTrue_WhenStatusIsActive()
    {
        // Arrange
        var user = new User { Status = UserStatus.Active };

        // Act
        var isActive = user.IsActive();

        // Assert
        isActive.Should().BeTrue();
    }

    [Fact]
    public void IsActive_ShouldReturnFalse_WhenStatusIsNotActive()
    {
        // Arrange
        var user = new User { Status = UserStatus.Inactive };

        // Act
        var isActive = user.IsActive();

        // Assert
        isActive.Should().BeFalse();
    }

    [Fact]
    public void RequirePasswordChange_ShouldSetFlagToTrue_AndUpdateTimestamp()
    {
        // Arrange
        var user = new User { RequiresPasswordChange = false };
        var beforeChange = DateTime.UtcNow;

        // Act
        user.RequirePasswordChange();

        // Assert
        user.RequiresPasswordChange.Should().BeTrue();
        user.UpdatedAt.Should().BeCloseTo(beforeChange, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void PasswordChanged_ShouldSetFlagToFalse_AndUpdateTimestamp()
    {
        // Arrange
        var user = new User { RequiresPasswordChange = true };
        var beforeChange = DateTime.UtcNow;

        // Act
        user.PasswordChanged();

        // Assert
        user.RequiresPasswordChange.Should().BeFalse();
        user.UpdatedAt.Should().BeCloseTo(beforeChange, TimeSpan.FromSeconds(1));
    }
}
