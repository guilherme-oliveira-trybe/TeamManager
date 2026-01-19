using FluentValidation.TestHelper;
using GFATeamManager.Application.DTOS.Sector;
using GFATeamManager.Application.Validators.Sector;

namespace GFATeamManager.Application.Tests.Validators;

public class SectorValidatorsTests
{
    [Fact]
    public void CreateSectorRequestValidator_ShouldPass_WhenDataIsValid()
    {
        // Arrange
        var validator = new CreateSectorRequestValidator();
        var request = new CreateSectorRequest
        {
            DepartmentId = Guid.NewGuid(),
            Name = "Fisioterapia",
            Description = "Setor de fisioterapia"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateSectorRequestValidator_ShouldFail_WhenDepartmentIdIsEmpty()
    {
        // Arrange
        var validator = new CreateSectorRequestValidator();
        var request = new CreateSectorRequest
        {
            DepartmentId = Guid.Empty,
            Name = "Fisioterapia"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DepartmentId)
            .WithErrorMessage("Departamento é obrigatório");
    }

    [Fact]
    public void CreateSectorRequestValidator_ShouldFail_WhenNameIsEmpty()
    {
        // Arrange
        var validator = new CreateSectorRequestValidator();
        var request = new CreateSectorRequest
        {
            DepartmentId = Guid.NewGuid(),
            Name = ""
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Nome é obrigatório");
    }

    [Fact]
    public void CreateSectorRequestValidator_ShouldFail_WhenNameIsTooShort()
    {
        // Arrange
        var validator = new CreateSectorRequestValidator();
        var request = new CreateSectorRequest
        {
            DepartmentId = Guid.NewGuid(),
            Name = "AB" // Too short (min is 3)
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Nome deve ter no mínimo 3 caracteres");
    }

    [Fact]
    public void CreateSectorRequestValidator_ShouldFail_WhenNameIsTooLong()
    {
        // Arrange
        var validator = new CreateSectorRequestValidator();
        var request = new CreateSectorRequest
        {
            DepartmentId = Guid.NewGuid(),
            Name = new string('A', 101) // 101 characters (max is 100)
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Nome deve ter no máximo 100 caracteres");
    }

    [Fact]
    public void CreateSectorRequestValidator_ShouldFail_WhenDescriptionIsTooLong()
    {
        // Arrange
        var validator = new CreateSectorRequestValidator();
        var request = new CreateSectorRequest
        {
            DepartmentId = Guid.NewGuid(),
            Name = "Valid Name",
            Description = new string('A', 501) // 501 characters (max is 500)
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Descrição deve ter no máximo 500 caracteres");
    }

    [Fact]
    public void UpdateSectorRequestValidator_ShouldPass_WhenDataIsValid()
    {
        // Arrange
        var validator = new UpdateSectorRequestValidator();
        var request = new UpdateSectorRequest
        {
            Name = "Updated Sector Name",
            Description = "Updated description"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void UpdateSectorRequestValidator_ShouldFail_WhenNameIsEmpty()
    {
        // Arrange
        var validator = new UpdateSectorRequestValidator();
        var request = new UpdateSectorRequest
        {
            Name = ""
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Nome é obrigatório");
    }

    [Fact]
    public void UpdateSectorRequestValidator_ShouldFail_WhenDescriptionIsTooLong()
    {
        // Arrange
        var validator = new UpdateSectorRequestValidator();
        var request = new UpdateSectorRequest
        {
            Name = "Valid Name",
            Description = new string('A', 501)
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Descrição deve ter no máximo 500 caracteres");
    }
}
