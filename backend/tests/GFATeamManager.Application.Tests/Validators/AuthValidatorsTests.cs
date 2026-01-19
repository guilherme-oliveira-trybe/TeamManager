using FluentValidation.TestHelper;
using GFATeamManager.Application.DTOS.Auth;
using GFATeamManager.Application.DTOs.Auth;
using GFATeamManager.Application.Validators.Auth;

namespace GFATeamManager.Application.Tests.Validators;

public class AuthValidatorsTests
{
    // ==================== LoginRequestValidator ====================
    
    [Fact]
    public void LoginRequestValidator_ShouldPass_WhenDataIsValid_WithEmail()
    {
        // Arrange
        var validator = new LoginRequestValidator();
        var request = new LoginRequest
        {
            Login = "user@example.com",
            Password = "Password123"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void LoginRequestValidator_ShouldPass_WhenDataIsValid_WithCpf()
    {
        // Arrange
        var validator = new LoginRequestValidator();
        var request = new LoginRequest
        {
            Login = "12345678909", // Valid CPF format
            Password = "Password123"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void LoginRequestValidator_ShouldFail_WhenLoginIsEmpty()
    {
        // Arrange
        var validator = new LoginRequestValidator();
        var request = new LoginRequest
        {
            Login = "",
            Password = "Password123"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Login)
            .WithErrorMessage("Login é obrigatório");
    }

    [Fact]
    public void LoginRequestValidator_ShouldFail_WhenLoginIsInvalid()
    {
        // Arrange
        var validator = new LoginRequestValidator();
        var request = new LoginRequest
        {
            Login = "invalid-login", // Not email or CPF
            Password = "Password123"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Login)
            .WithErrorMessage("Login deve ser um email válido ou CPF com 11 dígitos");
    }

    [Fact]
    public void LoginRequestValidator_ShouldFail_WhenPasswordIsTooShort()
    {
        // Arrange
        var validator = new LoginRequestValidator();
        var request = new LoginRequest
        {
            Login = "user@example.com",
            Password = "Pass123" // 7 characters
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Senha deve ter no mínimo 8 caracteres");
    }

    // ==================== ChangePasswordRequestValidator ====================

    [Fact]
    public void ChangePasswordRequestValidator_ShouldPass_WhenDataIsValid()
    {
        // Arrange
        var validator = new ChangePasswordRequestValidator();
        var request = new ChangePasswordRequest
        {
            CurrentPassword = "OldPassword123",
            NewPassword = "NewPassword123",
            ConfirmNewPassword = "NewPassword123"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ChangePasswordRequestValidator_ShouldFail_WhenCurrentPasswordIsEmpty()
    {
        // Arrange
        var validator = new ChangePasswordRequestValidator();
        var request = new ChangePasswordRequest
        {
            CurrentPassword = "",
            NewPassword = "NewPassword123",
            ConfirmNewPassword = "NewPassword123"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CurrentPassword)
            .WithErrorMessage("Senha atual é obrigatória");
    }

    [Fact]
    public void ChangePasswordRequestValidator_ShouldFail_WhenNewPasswordIsTooShort()
    {
        // Arrange
        var validator = new ChangePasswordRequestValidator();
        var request = new ChangePasswordRequest
        {
            CurrentPassword = "OldPassword123",
            NewPassword = "Pass1", // Too short
            ConfirmNewPassword = "Pass1"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("Nova senha deve ter no mínimo 8 caracteres");
    }

    [Fact]
    public void ChangePasswordRequestValidator_ShouldFail_WhenNewPasswordHasNoUppercase()
    {
        // Arrange
        var validator = new ChangePasswordRequestValidator();
        var request = new ChangePasswordRequest
        {
            CurrentPassword = "OldPassword123",
            NewPassword = "newpassword123", // No uppercase
            ConfirmNewPassword = "newpassword123"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("Nova senha deve conter pelo menos uma letra maiúscula");
    }

    [Fact]
    public void ChangePasswordRequestValidator_ShouldFail_WhenNewPasswordHasNoNumber()
    {
        // Arrange
        var validator = new ChangePasswordRequestValidator();
        var request = new ChangePasswordRequest
        {
            CurrentPassword = "OldPassword123",
            NewPassword = "NewPassword", // No number
            ConfirmNewPassword = "NewPassword"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithErrorMessage("Nova senha deve conter pelo menos um número");
    }

    [Fact]
    public void ChangePasswordRequestValidator_ShouldFail_WhenPasswordsDoNotMatch()
    {
        // Arrange
        var validator = new ChangePasswordRequestValidator();
        var request = new ChangePasswordRequest
        {
            CurrentPassword = "OldPassword123",
            NewPassword = "NewPassword123",
            ConfirmNewPassword = "DifferentPassword123"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ConfirmNewPassword)
            .WithErrorMessage("As senhas não coincidem");
    }

    // ==================== RequestPasswordResetRequestValidator ====================

    [Fact]
    public void RequestPasswordResetRequestValidator_ShouldPass_WhenDataIsValid()
    {
        // Arrange
        var validator = new RequestPasswordResetRequestValidator();
        var request = new RequestPasswordResetRequest
        {
            Cpf = "12345678909",
            Email = "user@example.com"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void RequestPasswordResetRequestValidator_ShouldFail_WhenCpfIsEmpty()
    {
        // Arrange
        var validator = new RequestPasswordResetRequestValidator();
        var request = new RequestPasswordResetRequest
        {
            Cpf = "",
            Email = "user@example.com"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cpf)
            .WithErrorMessage("CPF é obrigatório");
    }

    [Fact]
    public void RequestPasswordResetRequestValidator_ShouldFail_WhenCpfIsInvalid()
    {
        // Arrange
        var validator = new RequestPasswordResetRequestValidator();
        var request = new RequestPasswordResetRequest
        {
            Cpf = "123456789", // Invalid CPF
            Email = "user@example.com"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cpf)
            .WithErrorMessage("CPF inválido");
    }

    [Fact]
    public void RequestPasswordResetRequestValidator_ShouldFail_WhenEmailIsInvalid()
    {
        // Arrange
        var validator = new RequestPasswordResetRequestValidator();
        var request = new RequestPasswordResetRequest
        {
            Cpf = "12345678909",
            Email = "invalid-email"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email inválido");
    }
}
