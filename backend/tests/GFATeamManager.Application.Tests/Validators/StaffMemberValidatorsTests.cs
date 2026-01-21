using FluentValidation.TestHelper;
using GFATeamManager.Application.DTOS.StaffMember;
using GFATeamManager.Application.Validators.StaffMember;

namespace GFATeamManager.Application.Tests.Validators;

public class StaffMemberValidatorsTests
{
    [Fact]
    public void CreateStaffMemberRequestValidator_ShouldPass_WhenDataIsValid()
    {
        // Arrange
        var validator = new CreateStaffMemberRequestValidator();
        var request = new CreateStaffMemberRequest
        {
            SectorId = Guid.NewGuid(),
            FullName = "João Silva Santos",
            Email = "joao.silva@example.com",
            Phone = "11987654321",
            Specialty = "Fisioterapeuta Esportivo"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateStaffMemberRequestValidator_ShouldFail_WhenSectorIdIsEmpty()
    {
        // Arrange
        var validator = new CreateStaffMemberRequestValidator();
        var request = new CreateStaffMemberRequest
        {
            SectorId = Guid.Empty,
            FullName = "João Silva",
            Phone = "11987654321"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SectorId)
            .WithErrorMessage("Setor é obrigatório");
    }

    [Fact]
    public void CreateStaffMemberRequestValidator_ShouldFail_WhenFullNameIsEmpty()
    {
        // Arrange
        var validator = new CreateStaffMemberRequestValidator();
        var request = new CreateStaffMemberRequest
        {
            SectorId = Guid.NewGuid(),
            FullName = "",
            Phone = "11987654321"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage("Nome completo é obrigatório");
    }

    [Fact]
    public void CreateStaffMemberRequestValidator_ShouldFail_WhenFullNameIsTooShort()
    {
        // Arrange
        var validator = new CreateStaffMemberRequestValidator();
        var request = new CreateStaffMemberRequest
        {
            SectorId = Guid.NewGuid(),
            FullName = "AB", // Too short (min is 3)
            Phone = "11987654321"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage("Nome deve ter no mínimo 3 caracteres");
    }

    [Fact]
    public void CreateStaffMemberRequestValidator_ShouldFail_WhenFullNameIsTooLong()
    {
        // Arrange
        var validator = new CreateStaffMemberRequestValidator();
        var request = new CreateStaffMemberRequest
        {
            SectorId = Guid.NewGuid(),
            FullName = new string('A', 201), // 201 characters (max is 200)
            Phone = "11987654321"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage("Nome deve ter no máximo 200 caracteres");
    }

    [Fact]
    public void CreateStaffMemberRequestValidator_ShouldFail_WhenEmailIsInvalid()
    {
        // Arrange
        var validator = new CreateStaffMemberRequestValidator();
        var request = new CreateStaffMemberRequest
        {
            SectorId = Guid.NewGuid(),
            FullName = "João Silva",
            Email = "invalid-email", // Invalid format
            Phone = "11987654321"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email inválido");
    }

    [Fact]
    public void CreateStaffMemberRequestValidator_ShouldPass_WhenEmailIsNull()
    {
        // Arrange
        var validator = new CreateStaffMemberRequestValidator();
        var request = new CreateStaffMemberRequest
        {
            SectorId = Guid.NewGuid(),
            FullName = "João Silva",
            Email = null, // Optional
            Phone = "11987654321"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void CreateStaffMemberRequestValidator_ShouldFail_WhenPhoneIsEmpty()
    {
        // Arrange
        var validator = new CreateStaffMemberRequestValidator();
        var request = new CreateStaffMemberRequest
        {
            SectorId = Guid.NewGuid(),
            FullName = "João Silva",
            Phone = ""
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Phone)
            .WithErrorMessage("Telefone é obrigatório");
    }

    [Fact]
    public void CreateStaffMemberRequestValidator_ShouldFail_WhenPhoneIsTooShort()
    {
        // Arrange
        var validator = new CreateStaffMemberRequestValidator();
        var request = new CreateStaffMemberRequest
        {
            SectorId = Guid.NewGuid(),
            FullName = "João Silva",
            Phone = "123456789" // Too short (min is 10)
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Phone)
            .WithErrorMessage("Telefone deve ter no mínimo 10 dígitos");
    }

    [Fact]
    public void CreateStaffMemberRequestValidator_ShouldFail_WhenPhoneIsTooLong()
    {
        // Arrange
        var validator = new CreateStaffMemberRequestValidator();
        var request = new CreateStaffMemberRequest
        {
            SectorId = Guid.NewGuid(),
            FullName = "João Silva",
            Phone = new string('1', 21) // 21 characters (max is 20)
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Phone)
            .WithErrorMessage("Telefone deve ter no máximo 20 caracteres");
    }

    [Fact]
    public void CreateStaffMemberRequestValidator_ShouldFail_WhenSpecialtyIsTooLong()
    {
        // Arrange
        var validator = new CreateStaffMemberRequestValidator();
        var request = new CreateStaffMemberRequest
        {
            SectorId = Guid.NewGuid(),
            FullName = "João Silva",
            Phone = "11987654321",
            Specialty = new string('A', 101) // 101 characters (max is 100)
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Specialty)
            .WithErrorMessage("Especialidade deve ter no máximo 100 caracteres");
    }

    [Fact]
    public void UpdateStaffMemberRequestValidator_ShouldPass_WhenDataIsValid()
    {
        // Arrange
        var validator = new UpdateStaffMemberRequestValidator();
        var request = new UpdateStaffMemberRequest
        {
            FullName = "Maria Santos Silva",
            Email = "maria.santos@example.com",
            Phone = "21987654321",
            Specialty = "Nutricionista Esportiva"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void UpdateStaffMemberRequestValidator_ShouldFail_WhenFullNameIsEmpty()
    {
        // Arrange
        var validator = new UpdateStaffMemberRequestValidator();
        var request = new UpdateStaffMemberRequest
        {
            FullName = "",
            Phone = "11987654321"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName)
            .WithErrorMessage("Nome completo é obrigatório");
    }

    [Fact]
    public void UpdateStaffMemberRequestValidator_ShouldFail_WhenEmailIsInvalid()
    {
        // Arrange
        var validator = new UpdateStaffMemberRequestValidator();
        var request = new UpdateStaffMemberRequest
        {
            FullName = "João Silva",
            Email = "not-an-email",
            Phone = "11987654321"
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email inválido");
    }

    [Fact]
    public void UpdateStaffMemberRequestValidator_ShouldPass_WhenSpecialtyIsNull()
    {
        // Arrange
        var validator = new UpdateStaffMemberRequestValidator();
        var request = new UpdateStaffMemberRequest
        {
            FullName = "João Silva",
            Phone = "11987654321",
            Specialty = null // Optional
        };

        // Act
        var result = validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Specialty);
    }
}
