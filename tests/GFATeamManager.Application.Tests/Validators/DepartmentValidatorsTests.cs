using FluentValidation.TestHelper;
using GFATeamManager.Application.DTOS.Department;
using GFATeamManager.Application.Validators.Department;

namespace GFATeamManager.Application.Tests.Validators;

public class DepartmentValidatorsTests
{
    [Fact]
    public void CreateDepartmentRequestValidator_ShouldPass_WhenDataIsValid()
    {
        // Arrange
        var validator = new CreateDepartmentRequestValidator();
        var request = new CreateDepartmentRequest
        {
            Name = "Médico & Performance",
            Description = "Departamento responsável pela saúde dos atletas"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateDepartmentRequestValidator_ShouldFail_WhenNameIsEmpty()
    {
        // Arrange
        var validator = new CreateDepartmentRequestValidator();
        var request = new CreateDepartmentRequest
        {
            Name = "",
            Description = "Test"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Nome é obrigatório");
    }

    [Fact]
    public void CreateDepartmentRequestValidator_ShouldFail_WhenNameIsTooShort()
    {
        // Arrange
        var validator = new CreateDepartmentRequestValidator();
        var request = new CreateDepartmentRequest
        {
            Name = "AB", // Too short (min is 3)
            Description = "Test"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Nome deve ter no mínimo 3 caracteres");
    }

    [Fact]
    public void CreateDepartmentRequestValidator_ShouldFail_WhenNameIsTooLong()
    {
        // Arrange
        var validator = new CreateDepartmentRequestValidator();
        var request = new CreateDepartmentRequest
        {
            Name = new string('A', 101), // 101 characters (max is 100)
            Description = "Test"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Nome deve ter no máximo 100 caracteres");
    }

    [Fact]
    public void CreateDepartmentRequestValidator_ShouldFail_WhenDescriptionIsTooLong()
    {
        // Arrange
        var validator = new CreateDepartmentRequestValidator();
        var request = new CreateDepartmentRequest
        {
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
    public void CreateDepartmentRequestValidator_ShouldPass_WhenDescriptionIsNull()
    {
        // Arrange
        var validator = new CreateDepartmentRequestValidator();
        var request = new CreateDepartmentRequest
        {
            Name = "Valid Name",
            Description = null // Optional field
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void UpdateDepartmentRequestValidator_ShouldPass_WhenDataIsValid()
    {
        // Arrange
        var validator = new UpdateDepartmentRequestValidator();
        var request = new UpdateDepartmentRequest
        {
            Name = "Updated Department Name",
            Description = "Updated description"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void UpdateDepartmentRequestValidator_ShouldFail_WhenNameIsEmpty()
    {
        // Arrange
        var validator = new UpdateDepartmentRequestValidator();
        var request = new UpdateDepartmentRequest
        {
            Name = "",
            Description = "Test"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Nome é obrigatório");
    }

    [Fact]
    public void UpdateDepartmentRequestValidator_ShouldFail_WhenDescriptionIsTooLong()
    {
        // Arrange
        var validator = new UpdateDepartmentRequestValidator();
        var request = new UpdateDepartmentRequest
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
