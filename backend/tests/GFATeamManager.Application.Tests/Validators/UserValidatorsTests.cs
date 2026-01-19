using FluentValidation.TestHelper;
using GFATeamManager.Application.DTOS.User;
using GFATeamManager.Application.Validators.User;

namespace GFATeamManager.Application.Tests.Validators;

public class UserValidatorsTests
{
    // ==================== CompleteRegistrationRequestValidator ====================

    [Fact]
    public void CompleteRegistrationRequestValidator_ShouldPass_WhenDataIsValid()
    {
        // Arrange
        var validator = new CompleteRegistrationRequestValidator();
        var request = new CompleteRegistrationRequest
        {
            Cpf = "12345678909",
            ActivationCode = "ABC12345",
            FullName = "John Doe Test",
            BirthDate = DateTime.Now.AddYears(-20),
            Weight = 75.5m,
            Height = 180,
            Phone = "11999999999",
            Email = "john@example.com",
            Password = "Password123",
            ConfirmPassword = "Password123",
            EmergencyContactName = "Jane Doe",
            EmergencyContactPhone = "11988888888"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CompleteRegistrationRequestValidator_ShouldFail_WhenCpfIsInvalid()
    {
        // Arrange
        var validator = new CompleteRegistrationRequestValidator();
        var request = new CompleteRegistrationRequest
        {
            Cpf = "123", // Invalid CPF
            ActivationCode = "ABC12345",
            FullName = "John Doe",
            BirthDate = DateTime.Now.AddYears(-20),
            Weight = 75.5m,
            Height = 180,
            Phone = "11999999999",
            Email = "john@example.com",
            Password = "Password123",
            ConfirmPassword = "Password123",
            EmergencyContactName = "Jane Doe",
            EmergencyContactPhone = "11988888888"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cpf)
            .WithErrorMessage("CPF inválido");
    }

    [Fact]
    public void CompleteRegistrationRequestValidator_ShouldFail_WhenActivationCodeIsWrongLength()
    {
        // Arrange
        var validator = new CompleteRegistrationRequestValidator();
        var request = new CompleteRegistrationRequest
        {
            Cpf = "12345678909",
            ActivationCode = "ABC123", // Too short
            FullName = "John Doe",
            BirthDate = DateTime.Now.AddYears(-20),
            Weight = 75.5m,
            Height = 180,
            Phone = "11999999999",
            Email = "john@example.com",
            Password = "Password123",
            ConfirmPassword = "Password123",
            EmergencyContactName = "Jane Doe",
            EmergencyContactPhone = "11988888888"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ActivationCode)
            .WithErrorMessage("Código deve ter 8 caracteres");
    }

    [Fact]
    public void CompleteRegistrationRequestValidator_ShouldFail_WhenUserIsTooYoung()
    {
        // Arrange
        var validator = new CompleteRegistrationRequestValidator();
        var request = new CompleteRegistrationRequest
        {
            Cpf = "12345678909",
            ActivationCode = "ABC12345",
            FullName = "Young User",
            BirthDate = DateTime.Now.AddYears(-10), // Too young
            Weight = 50,
            Height = 150,
            Phone = "11999999999",
            Email = "young@example.com",
            Password = "Password123",
            ConfirmPassword = "Password123",
            EmergencyContactName = "Parent",
            EmergencyContactPhone = "11988888888"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BirthDate)
            .WithErrorMessage("Usuário deve ter pelo menos 13 anos");
    }

    [Fact]
    public void CompleteRegistrationRequestValidator_ShouldFail_WhenPasswordHasNoUppercase()
    {
        // Arrange
        var validator = new CompleteRegistrationRequestValidator();
        var request = new CompleteRegistrationRequest
        {
            Cpf = "12345678909",
            ActivationCode = "ABC12345",
            FullName = "John Doe",
            BirthDate = DateTime.Now.AddYears(-20),
            Weight = 75.5m,
            Height = 180,
            Phone = "11999999999",
            Email = "john@example.com",
            Password = "password123", // No uppercase
            ConfirmPassword = "password123",
            EmergencyContactName = "Jane Doe",
            EmergencyContactPhone = "11988888888"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Senha deve conter pelo menos uma letra maiúscula");
    }

    [Fact]
    public void CompleteRegistrationRequestValidator_ShouldFail_WhenPasswordsDoNotMatch()
    {
        // Arrange
        var validator = new CompleteRegistrationRequestValidator();
        var request = new CompleteRegistrationRequest
        {
            Cpf = "12345678909",
            ActivationCode = "ABC12345",
            FullName = "John Doe",
            BirthDate = DateTime.Now.AddYears(-20),
            Weight = 75.5m,
            Height = 180,
            Phone = "11999999999",
            Email = "john@example.com",
            Password = "Password123",
            ConfirmPassword = "DifferentPassword123", // Different
            EmergencyContactName = "Jane Doe",
            EmergencyContactPhone = "11988888888"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword)
            .WithErrorMessage("As senhas não coincidem");
    }

    [Fact]
    public void CompleteRegistrationRequestValidator_ShouldFail_WhenWeightIsInvalid()
    {
        // Arrange
        var validator = new CompleteRegistrationRequestValidator();
        var request = new CompleteRegistrationRequest
        {
            Cpf = "12345678909",
            ActivationCode = "ABC12345",
            FullName = "John Doe",
            BirthDate = DateTime.Now.AddYears(-20),
            Weight = 350, // Too heavy
            Height = 180,
            Phone = "11999999999",
            Email = "john@example.com",
            Password = "Password123",
            ConfirmPassword = "Password123",
            EmergencyContactName = "Jane Doe",
            EmergencyContactPhone = "11988888888"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Weight)
            .WithErrorMessage("Peso deve ser menor que 300kg");
    }

    // ==================== UpdateUserRequestValidator ====================

    [Fact]
    public void UpdateUserRequestValidator_ShouldPass_WhenDataIsValid()
    {
        // Arrange
        var validator = new UpdateUserRequestValidator();
        var request = new UpdateUserRequest
        {
            FullName = "John Doe Updated",
            BirthDate = DateTime.Now.AddYears(-25),
            Weight = 80,
            Height = 185,
            Phone = "11999999999",
            Email = "johnupdated@example.com",
            EmergencyContactName = "Jane Doe",
            EmergencyContactPhone = "11988888888"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void UpdateUserRequestValidator_ShouldPass_WhenEmergencyContactIsEmpty()
    {
        // Arrange
        var validator = new UpdateUserRequestValidator();
        var request = new UpdateUserRequest
        {
            FullName = "John Doe",
            BirthDate = DateTime.Now.AddYears(-25),
            Weight = 80,
            Height = 185,
            Phone = "11999999999",
            Email = "john@example.com",
            EmergencyContactName = null,
            EmergencyContactPhone = null
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void UpdateUserRequestValidator_ShouldFail_WhenFullNameIsTooShort()
    {
        // Arrange
        var validator = new UpdateUserRequestValidator();
        var request = new UpdateUserRequest
        {
            FullName = "Jo", // Too short
            BirthDate = DateTime.Now.AddYears(-25),
            Weight = 80,
            Height = 185,
            Phone = "11999999999",
            Email = "john@example.com"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage("Nome deve ter no mínimo 3 caracteres");
    }

    [Fact]
    public void UpdateUserRequestValidator_ShouldFail_WhenEmailIsInvalid()
    {
        // Arrange
        var validator = new UpdateUserRequestValidator();
        var request = new UpdateUserRequest
        {
            FullName = "John Doe",
            BirthDate = DateTime.Now.AddYears(-25),
            Weight = 80,
            Height = 185,
            Phone = "11999999999",
            Email = "invalid-email" // Invalid
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email inválido");
    }

    [Fact]
    public void UpdateUserRequestValidator_ShouldFail_WhenHeightIsTooHigh()
    {
        // Arrange
        var validator = new UpdateUserRequestValidator();
        var request = new UpdateUserRequest
        {
            FullName = "John Doe",
            BirthDate = DateTime.Now.AddYears(-25),
            Weight = 80,
            Height = 300, // Too tall
            Phone = "11999999999",
            Email = "john@example.com"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Height)
            .WithErrorMessage("Altura deve ser menor que 250cm");
    }

    [Fact]
    public void UpdateUserRequestValidator_ShouldFail_WhenEmergencyContactNameIsTooShort()
    {
        // Arrange
        var validator = new UpdateUserRequestValidator();
        var request = new UpdateUserRequest
        {
            FullName = "John Doe",
            BirthDate = DateTime.Now.AddYears(-25),
            Weight = 80,
            Height = 185,
            Phone = "11999999999",
            Email = "john@example.com",
            EmergencyContactName = "AB", // Too short
            EmergencyContactPhone = "11988888888"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EmergencyContactName)
            .WithErrorMessage("Nome do contato deve ter no mínimo 3 caracteres");
    }
}
