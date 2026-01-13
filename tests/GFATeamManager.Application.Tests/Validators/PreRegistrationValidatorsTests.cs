using FluentValidation.TestHelper;
using GFATeamManager.Application.DTOS.PreRegistration;
using GFATeamManager.Application.Validators.PreRegistration;
using GFATeamManager.Domain.Enums;

namespace GFATeamManager.Application.Tests.Validators;

public class PreRegistrationValidatorsTests
{
    [Fact]
    public void CreatePreRegistrationValidator_ShouldPass_WhenDataIsValid()
    {
        // Arrange
        var validator = new CreatePreRegistrationRequestValidator();
        var request = new CreatePreRegistrationRequest
        {
            Cpf = "12345678909",
            Profile = ProfileType.Athlete,
            Unit = PlayerUnit.Offense,
            Position = PlayerPosition.QB
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreatePreRegistrationValidator_ShouldPass_WhenUnitAndPositionAreNull()
    {
        // Arrange
        var validator = new CreatePreRegistrationRequestValidator();
        var request = new CreatePreRegistrationRequest
        {
            Cpf = "12345678909",
            Profile = ProfileType.Admin,
            Unit = null,
            Position = null
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreatePreRegistrationValidator_ShouldFail_WhenCpfIsEmpty()
    {
        // Arrange
        var validator = new CreatePreRegistrationRequestValidator();
        var request = new CreatePreRegistrationRequest
        {
            Cpf = "",
            Profile = ProfileType.Athlete
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cpf)
            .WithErrorMessage("CPF é obrigatório");
    }

    [Fact]
    public void CreatePreRegistrationValidator_ShouldFail_WhenCpfIsInvalid()
    {
        // Arrange
        var validator = new CreatePreRegistrationRequestValidator();
        var request = new CreatePreRegistrationRequest
        {
            Cpf = "123456789", // Invalid CPF
            Profile = ProfileType.Athlete
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cpf)
            .WithErrorMessage("CPF inválido");
    }

    [Fact]
    public void CreatePreRegistrationValidator_ShouldFail_WhenProfileIsInvalid()
    {
        // Arrange
        var validator = new CreatePreRegistrationRequestValidator();
        var request = new CreatePreRegistrationRequest
        {
            Cpf = "12345678909",
            Profile = (ProfileType)999 // Invalid enum value
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Profile);
    }
}
