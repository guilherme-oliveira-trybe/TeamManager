using FluentAssertions;
using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Enums;
using Xunit;

namespace GFATeamManager.Domain.Tests.Entities;

public class PreRegistrationTests
{
    [Fact]
    public void Constructor_ShouldGenerateActivationCode_AndSetExpirationDate()
    {
        // Act
        var preRegistration = new PreRegistration();

        // Assert
        preRegistration.ActivationCode.Should().NotBeNullOrEmpty();
        preRegistration.ActivationCode.Length.Should().Be(8);
        preRegistration.ExpirationDate.Should().BeCloseTo(DateTime.UtcNow.AddDays(7), TimeSpan.FromMinutes(1));
        preRegistration.IsUsed.Should().BeFalse();
    }

    [Fact]
    public void IsExpired_ShouldReturnTrue_WhenExpirationDateIsPast()
    {
        // Arrange
        var preRegistration = new PreRegistration
        {
            ExpirationDate = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var isExpired = preRegistration.IsExpired();

        // Assert
        isExpired.Should().BeTrue();
    }

    [Fact]
    public void IsExpired_ShouldReturnFalse_WhenExpirationDateIsFuture()
    {
        // Arrange
        var preRegistration = new PreRegistration
        {
            ExpirationDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var isExpired = preRegistration.IsExpired();

        // Assert
        isExpired.Should().BeFalse();
    }

    [Fact]
    public void CanBeUsed_ShouldReturnTrue_WhenNotUsedAndNotExpired()
    {
        // Arrange
        var preRegistration = new PreRegistration
        {
            IsUsed = false,
            ExpirationDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var canBeUsed = preRegistration.CanBeUsed();

        // Assert
        canBeUsed.Should().BeTrue();
    }

    [Fact]
    public void CanBeUsed_ShouldReturnFalse_WhenAlreadyUsed()
    {
        // Arrange
        var preRegistration = new PreRegistration
        {
            IsUsed = true,
            ExpirationDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var canBeUsed = preRegistration.CanBeUsed();

        // Assert
        canBeUsed.Should().BeFalse();
    }

    [Fact]
    public void CanBeUsed_ShouldReturnFalse_WhenExpired()
    {
        // Arrange
        var preRegistration = new PreRegistration
        {
            IsUsed = false,
            ExpirationDate = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var canBeUsed = preRegistration.CanBeUsed();

        // Assert
        canBeUsed.Should().BeFalse();
    }
}
