using FluentValidation.TestHelper;
using GFATeamManager.Application.DTOS.Activities;
using GFATeamManager.Application.Validators.Activities;
using GFATeamManager.Domain.Enums;

namespace GFATeamManager.Application.Tests.Validators;

public class ActivityValidatorsTests
{
    [Fact]
    public void CreateActivityRequestValidator_ShouldPass_WhenDataIsValid()
    {
        // Arrange
        var validator = new CreateActivityRequestValidator();
        var request = new CreateActivityRequest
        {
            Type = ActivityType.Training,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddHours(2),
            Location = "Valid Location",
            TargetUnit = PlayerUnit.Offense
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateActivityRequestValidator_ShouldFail_WhenLocationIsTooShort()
    {
        // Arrange
        var validator = new CreateActivityRequestValidator();
        var request = new CreateActivityRequest
        {
            Type = ActivityType.Training,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddHours(2),
            Location = "AB", // Too short
            TargetUnit = PlayerUnit.Offense
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Location)
            .WithErrorMessage("Local deve ter no mínimo 3 caracteres");
    }

    [Fact]
    public void CreateActivityRequestValidator_ShouldFail_WhenEndDateIsBeforeStartDate()
    {
        // Arrange
        var validator = new CreateActivityRequestValidator();
        var request = new CreateActivityRequest
        {
            Type = ActivityType.Training,
            StartDate = DateTime.UtcNow.AddHours(2),
            EndDate = DateTime.UtcNow, // Before start date
            Location = "Valid Location"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EndDate)
            .WithErrorMessage("Data de término deve ser maior ou igual à data de início");
    }

    [Fact]
    public void CreateActivityItemRequestValidator_ShouldPass_WhenDataIsValid()
    {
        // Arrange
        var validator = new CreateActivityItemRequestValidator();
        var request = new CreateActivityItemRequest
        {
            Title = "Valid Title",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1),
            TargetUnit = PlayerUnit.Defense,
            TargetPositions = new List<PlayerPosition> { PlayerPosition.OL }
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateActivityItemRequestValidator_ShouldFail_WhenTitleIsEmpty()
    {
        // Arrange
        var validator = new CreateActivityItemRequestValidator();
        var request = new CreateActivityItemRequest
        {
            Title = "", // Empty
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1)
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Título é obrigatório");
    }

    [Fact]
    public void CreateActivityItemRequestValidator_ShouldFail_WhenEndTimeIsBeforeStartTime()
    {
        // Arrange
        var validator = new CreateActivityItemRequestValidator();
        var request = new CreateActivityItemRequest
        {
            Title = "Valid Title",
            StartTime = DateTime.UtcNow.AddHours(2),
            EndTime = DateTime.UtcNow // Before start time
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EndTime)
            .WithErrorMessage("Horário de término deve ser maior ou igual ao horário de início");
    }

    [Fact]
    public void UpdateActivityRequestValidator_ShouldPass_WhenDataIsValid()
    {
        // Arrange
        var validator = new UpdateActivityRequestValidator();
        var request = new UpdateActivityRequest
        {
            Type = ActivityType.Game,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddHours(3),
            Location = "Updated Location",
            TargetUnit = null
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void UpdateActivityItemRequestValidator_ShouldPass_WhenDataIsValid()
    {
        // Arrange
        var validator = new UpdateActivityItemRequestValidator();
        var request = new UpdateActivityItemRequest
        {
            Title = "Updated Title",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddMinutes(30),
            TargetUnit = PlayerUnit.Offense,
            TargetPositions = new List<PlayerPosition> { PlayerPosition.QB, PlayerPosition.RB }
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateActivityRequestValidator_ShouldFail_WhenLocationIsTooLong()
    {
        // Arrange
        var validator = new CreateActivityRequestValidator();
        var request = new CreateActivityRequest
        {
            Type = ActivityType.Training,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddHours(2),
            Location = new string('A', 201) // 201 characters (max is 200)
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Location);
    }

    [Fact]
    public void CreateActivityItemRequestValidator_ShouldFail_WhenTitleIsTooLong()
    {
        // Arrange
        var validator = new CreateActivityItemRequestValidator();
        var request = new CreateActivityItemRequest
        {
            Title = new string('A', 201), // 201 characters (max is 200)
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1)
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }
}
