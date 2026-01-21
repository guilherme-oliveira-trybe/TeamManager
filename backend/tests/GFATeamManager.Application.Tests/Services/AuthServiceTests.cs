using FluentAssertions;
using GFATeamManager.Application.DTOS.Auth;
using GFATeamManager.Application.Services;
using GFATeamManager.Application.Services.Interfaces;
using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Enums;
using GFATeamManager.Domain.Interfaces.Repositories;
using Moq;
using Xunit;

namespace GFATeamManager.Application.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<IPasswordResetRequestRepository> _passwordResetRequestRepositoryMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _jwtServiceMock = new Mock<IJwtService>();
        _passwordResetRequestRepositoryMock = new Mock<IPasswordResetRequestRepository>();
        
        _authService = new AuthService(
            _userRepositoryMock.Object,
            _jwtServiceMock.Object,
            _passwordResetRequestRepositoryMock.Object
        );
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var request = new LoginRequest { Login = "wrong@email.com", Password = "password" };
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Login))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Login ou senha inválidos");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenPasswordIsInvalid()
    {
        // Arrange
        var request = new LoginRequest { Login = "user@email.com", Password = "wrongpassword" };
        var user = new User
        {
            Email = request.Login,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
            Status = UserStatus.Active
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Login))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Login ou senha inválidos");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenUserIsNotActive()
    {
        // Arrange
        var request = new LoginRequest { Login = "user@email.com", Password = "password" };
        var user = new User
        {
            Email = request.Login,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Status = UserStatus.AwaitingActivation
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Login))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Usuário não está ativo. Entre em contato com o administrador.");
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnSuccess_WhenCredentialsAreValidAndUserIsActive()
    {
        // Arrange
        var request = new LoginRequest { Login = "user@email.com", Password = "password" };
        var user = new User
        {
            Email = request.Login,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Status = UserStatus.Active
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Login))
            .ReturnsAsync(user);
        
        _jwtServiceMock.Setup(j => j.GenerateToken(user))
            .Returns("valid_token");
        
        _passwordResetRequestRepositoryMock
            .Setup(r => r.GetActiveRequestByUserIdAsync(user.Id))
            .ReturnsAsync((PasswordResetRequest?)null);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Token.Should().Be("valid_token");
        result.Data.RequiresPasswordChange.Should().BeFalse();
    }
    
    [Fact]
    public async Task LoginAsync_ShouldSoftDeletePendingRequest_WhenLoggingInWithNormalPassword()
    {
        // Arrange
        var request = new LoginRequest { Login = "user@email.com", Password = "password" };
        var user = new User
        {
            Email = request.Login,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Status = UserStatus.Active
        };
        
        var pendingRequest = new PasswordResetRequest
        {
            UserId = user.Id,
           ApprovedAt = null  // PENDING
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Login))
            .ReturnsAsync(user);
        
        _passwordResetRequestRepositoryMock
            .Setup(r => r.GetActiveRequestByUserIdAsync(user.Id))
            .ReturnsAsync(pendingRequest);
        
        _jwtServiceMock.Setup(j => j.GenerateToken(user))
            .Returns("valid_token");

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        pendingRequest.IsDeleted.Should().BeTrue();
        pendingRequest.DeletedAt.Should().NotBeNull();
        _passwordResetRequestRepositoryMock.Verify(
            r => r.UpdateAsync(pendingRequest),
            Times.Once);
    }
    
    [Fact]
    public async Task LoginAsync_ShouldBlockLogin_WhenApprovedRequestExists()
    {
        // Arrange
        var request = new LoginRequest { Login = "user@email.com", Password = "password" };
        var user = new User
        {
            Email = request.Login,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Status = UserStatus.Active
        };
        
        var approvedRequest = new PasswordResetRequest
        {
            UserId = user.Id
        };
        approvedRequest.Approve(Guid.NewGuid());  // APPROVED

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Login))
            .ReturnsAsync(user);
        
        _passwordResetRequestRepositoryMock
            .Setup(r => r.GetActiveRequestByUserIdAsync(user.Id))
            .ReturnsAsync(approvedRequest);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("senha temporária"));
        approvedRequest.IsDeleted.Should().BeFalse();
    }
    
    [Fact]
    public async Task LoginAsync_ShouldSucceed_WithTemporaryPassword()
    {
        // Arrange
        var user = new User
        {
            Email = "user@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldpassword"),
            Status = UserStatus.Active
        };
        user.RequirePasswordChange();
        
        var approvedRequest = new PasswordResetRequest
        {
            UserId = user.Id
        };
        // Approve() generates temp password and returns it
        var tempPassword = approvedRequest.Approve(Guid.NewGuid());
        
        var request = new LoginRequest { Login = user.Email, Password = tempPassword };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Login))
            .ReturnsAsync(user);
        
        _passwordResetRequestRepositoryMock
            .Setup(r => r.GetValidRequestByUserIdAsync(user.Id))
            .ReturnsAsync(approvedRequest);
        

        
        _jwtServiceMock.Setup(j => j.GenerateToken(user))
            .Returns("valid_token");

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.RequiresPasswordChange.Should().BeTrue();
        approvedRequest.IsUsed.Should().BeFalse();
    }
    
    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenTempPasswordExpired()
    {
        var user = new User
        {
            Email = "user@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldpassword"),
            Status = UserStatus.Active
        };
        user.RequirePasswordChange();
        
        var expiredRequest = new PasswordResetRequest
        {
            UserId = user.Id,
            ApprovedAt = DateTime.UtcNow.AddDays(-2),
            ExpirationDate = DateTime.UtcNow.AddDays(-1),
            TemporaryPasswordHash = BCrypt.Net.BCrypt.HashPassword("TEMPPASS123")
        };
        
        var request = new LoginRequest { Login = user.Email, Password = "TEMPPASS123" };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Login))
            .ReturnsAsync(user);
        
        _passwordResetRequestRepositoryMock
            .Setup(r => r.GetValidRequestByUserIdAsync(user.Id))
            .ReturnsAsync((PasswordResetRequest?)null);

        var result = await _authService.LoginAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Login ou senha inválidos");
    }
    
    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenTempPasswordAlreadyUsed()
    {
        var user = new User
        {
            Email = "user@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("newpassword"),
            Status = UserStatus.Active
        };
        
        var usedRequest = new PasswordResetRequest
        {
            UserId = user.Id,
            ApprovedAt = DateTime.UtcNow.AddHours(-1),
            ExpirationDate = DateTime.UtcNow.AddHours(23),
            TemporaryPasswordHash = BCrypt.Net.BCrypt.HashPassword("TEMPPASS123"),
            IsUsed = true,
            UsedAt = DateTime.UtcNow.AddMinutes(-30)
        };
        
        var request = new LoginRequest { Login = user.Email, Password = "TEMPPASS123" };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Login))
            .ReturnsAsync(user);
        
        _passwordResetRequestRepositoryMock
            .Setup(r => r.GetValidRequestByUserIdAsync(user.Id))
            .ReturnsAsync((PasswordResetRequest?)null);

        var result = await _authService.LoginAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Login ou senha inválidos");
    }
    
    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenTempPasswordInvalid()
    {
        var user = new User
        {
            Email = "user@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldpassword"),
            Status = UserStatus.Active
        };
        user.RequirePasswordChange();
        
        var approvedRequest = new PasswordResetRequest
        {
            UserId = user.Id
        };
        approvedRequest.Approve(Guid.NewGuid());
        
        var request = new LoginRequest { Login = user.Email, Password = "WRONGTEMPPASS" };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.Login))
            .ReturnsAsync(user);
        
        _passwordResetRequestRepositoryMock
            .Setup(r => r.GetValidRequestByUserIdAsync(user.Id))
            .ReturnsAsync(approvedRequest);

        var result = await _authService.LoginAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Login ou senha inválidos");
    }
    
    [Fact]
    public async Task ChangePasswordAsync_ShouldAcceptTemporaryPassword()
    {
        var user = new User
        {
            Email = "user@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldpassword"),
            Status = UserStatus.Active
        };
        user.RequirePasswordChange();
        
        var approvedRequest = new PasswordResetRequest
        {
            UserId = user.Id
        };
        var tempPassword = approvedRequest.Approve(Guid.NewGuid());
        
        var changeRequest = new ChangePasswordRequest
        {
            CurrentPassword = tempPassword,
            NewPassword = "NewSecurePass123",
            ConfirmNewPassword = "NewSecurePass123"
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id))
            .ReturnsAsync(user);
        
        _passwordResetRequestRepositoryMock
            .Setup(r => r.GetValidRequestByUserIdAsync(user.Id))
            .ReturnsAsync(approvedRequest);

        var result = await _authService.ChangePasswordAsync(user.Id, changeRequest);

        result.IsSuccess.Should().BeTrue();
    }
    
    [Fact]
    public async Task ChangePasswordAsync_ShouldMarkRequestAsUsed_WhenUsingTempPassword()
    {
        var user = new User
        {
            Email = "user@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldpassword"),
            Status = UserStatus.Active
        };
        user.RequirePasswordChange();
        
        var approvedRequest = new PasswordResetRequest
        {
            UserId = user.Id
        };
        var tempPassword = approvedRequest.Approve(Guid.NewGuid());
        
        var changeRequest = new ChangePasswordRequest
        {
            CurrentPassword = tempPassword,
            NewPassword = "NewSecurePass123",
            ConfirmNewPassword = "NewSecurePass123"
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id))
            .ReturnsAsync(user);
        
        _passwordResetRequestRepositoryMock
            .Setup(r => r.GetValidRequestByUserIdAsync(user.Id))
            .ReturnsAsync(approvedRequest);

        await _authService.ChangePasswordAsync(user.Id, changeRequest);

        approvedRequest.IsUsed.Should().BeTrue();
        approvedRequest.UsedAt.Should().NotBeNull();
        _passwordResetRequestRepositoryMock.Verify(
            r => r.UpdateAsync(approvedRequest),
            Times.Once);
    }
    
    [Fact]
    public async Task ChangePasswordAsync_ShouldClearRequiresPasswordChange_WhenUsingTempPassword()
    {
        var user = new User
        {
            Email = "user@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldpassword"),
            Status = UserStatus.Active
        };
        user.RequirePasswordChange();
        
        var approvedRequest = new PasswordResetRequest
        {
            UserId = user.Id
        };
        var tempPassword = approvedRequest.Approve(Guid.NewGuid());
        
        var changeRequest = new ChangePasswordRequest
        {
            CurrentPassword = tempPassword,
            NewPassword = "NewSecurePass123",
            ConfirmNewPassword = "NewSecurePass123"
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id))
            .ReturnsAsync(user);
        
        _passwordResetRequestRepositoryMock
            .Setup(r => r.GetValidRequestByUserIdAsync(user.Id))
            .ReturnsAsync(approvedRequest);

        await _authService.ChangePasswordAsync(user.Id, changeRequest);

        user.RequiresPasswordChange.Should().BeFalse();
        _userRepositoryMock.Verify(
            r => r.UpdateAsync(user),
            Times.Once);
    }
    
    [Fact]
    public async Task ChangePasswordAsync_ShouldRejectInvalidTemporaryPassword()
    {
        var user = new User
        {
            Email = "user@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldpassword"),
            Status = UserStatus.Active
        };
        user.RequirePasswordChange();
        
        var approvedRequest = new PasswordResetRequest
        {
            UserId = user.Id
        };
        approvedRequest.Approve(Guid.NewGuid());
        
        var changeRequest = new ChangePasswordRequest
        {
            CurrentPassword = "WRONGTEMPPASS",
            NewPassword = "NewSecurePass123",
            ConfirmNewPassword = "NewSecurePass123"
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id))
            .ReturnsAsync(user);
        
        _passwordResetRequestRepositoryMock
            .Setup(r => r.GetValidRequestByUserIdAsync(user.Id))
            .ReturnsAsync(approvedRequest);

        var result = await _authService.ChangePasswordAsync(user.Id, changeRequest);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Senha atual incorreta");
    }
    
    [Fact]
    public async Task ChangePasswordAsync_ShouldWorkWithNormalPassword_WhenNoResetRequest()
    {
        var user = new User
        {
            Email = "user@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldpassword"),
            Status = UserStatus.Active
        };
        
        var changeRequest = new ChangePasswordRequest
        {
            CurrentPassword = "oldpassword",
            NewPassword = "NewSecurePass123",
            ConfirmNewPassword = "NewSecurePass123"
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id))
            .ReturnsAsync(user);
        
        _passwordResetRequestRepositoryMock
            .Setup(r => r.GetValidRequestByUserIdAsync(user.Id))
            .ReturnsAsync((PasswordResetRequest?)null);

        var result = await _authService.ChangePasswordAsync(user.Id, changeRequest);

        result.IsSuccess.Should().BeTrue();
        user.RequiresPasswordChange.Should().BeFalse();
    }
    
    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnFailure_WhenPasswordsDoNotMatch()
    {
        var user = new User
        {
            Email = "user@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldpassword"),
            Status = UserStatus.Active
        };
        
        var changeRequest = new ChangePasswordRequest
        {
            CurrentPassword = "oldpassword",
            NewPassword = "NewSecurePass123",
            ConfirmNewPassword = "DifferentPassword"
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

        var result = await _authService.ChangePasswordAsync(user.Id, changeRequest);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("As senhas não coincidem");
    }
    
    [Fact]
    public async Task ChangePasswordAsync_ShouldReturnFailure_WhenNewPasswordTooShort()
    {
        var user = new User
        {
            Email = "user@email.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldpassword"),
            Status = UserStatus.Active
        };
        
        var changeRequest = new ChangePasswordRequest
        {
            CurrentPassword = "oldpassword",
            NewPassword = "short",
            ConfirmNewPassword = "short"
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

        var result = await _authService.ChangePasswordAsync(user.Id, changeRequest);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("8 caracteres"));
    }
    
    [Fact]
    public async Task RequestPasswordResetAsync_ShouldReturnSuccess_WhenUserExists()
    {
        var cpf = "61120319064";
        var email = "user@email.com";
        var user = new User
        {
            Cpf = cpf,
            Email = email,
            Status = UserStatus.Active
        };
        
        var request = new RequestPasswordResetRequest
        {
            Cpf = cpf,
            Email = email
        };

        _userRepositoryMock.Setup(r => r.GetByCpfAsync(cpf))
            .ReturnsAsync(user);
        
        _passwordResetRequestRepositoryMock
            .Setup(r => r.GetPendingRequestsAsync())
            .ReturnsAsync(new List<PasswordResetRequest>());

        var result = await _authService.RequestPasswordResetAsync(request);

        result.IsSuccess.Should().BeTrue();
        _passwordResetRequestRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<PasswordResetRequest>()),
            Times.Once);
    }
    
    [Fact]
    public async Task RequestPasswordResetAsync_ShouldReturnSuccess_WhenUserNotFound()
    {
        var cpf = "61120319064";
        var email = "wrong@email.com";
        
        var request = new RequestPasswordResetRequest
        {
            Cpf = cpf,
            Email = email
        };

        _userRepositoryMock.Setup(r => r.GetByCpfAsync(cpf))
            .ReturnsAsync((User?)null);

        var result = await _authService.RequestPasswordResetAsync(request);

        result.IsSuccess.Should().BeTrue();
        _passwordResetRequestRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<PasswordResetRequest>()),
            Times.Never);
    }
    
    [Fact]
    public async Task RequestPasswordResetAsync_ShouldReturnSuccess_WhenEmailDoesNotMatch()
    {
        var cpf = "61120319064";
        var user = new User
        {
            Cpf = cpf,
            Email = "correct@email.com",
            Status = UserStatus.Active
        };
        
        var request = new RequestPasswordResetRequest
        {
            Cpf = cpf,
            Email = "wrong@email.com"
        };

        _userRepositoryMock.Setup(r => r.GetByCpfAsync(cpf))
            .ReturnsAsync(user);

        var result = await _authService.RequestPasswordResetAsync(request);

        result.IsSuccess.Should().BeTrue();
        _passwordResetRequestRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<PasswordResetRequest>()),
            Times.Never);
    }
    
    [Fact]
    public async Task RequestPasswordResetAsync_ShouldReturnFailure_WhenUserAlreadyHasPendingRequest()
    {
        var cpf = "61120319064";
        var email = "user@email.com";
        var user = new User
        {
            Cpf = cpf,
            Email = email,
            Status = UserStatus.Active
        };
        
        var existingRequest = new PasswordResetRequest
        {
            UserId = user.Id,
            ApprovedAt = null
        };
        
        var request = new RequestPasswordResetRequest
        {
            Cpf = cpf,
            Email = email
        };

        _userRepositoryMock.Setup(r => r.GetByCpfAsync(cpf))
            .ReturnsAsync(user);
        
        _passwordResetRequestRepositoryMock
            .Setup(r => r.GetPendingRequestsAsync())
            .ReturnsAsync(new List<PasswordResetRequest> { existingRequest });

        var result = await _authService.RequestPasswordResetAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("solicitação pendente"));
        _passwordResetRequestRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<PasswordResetRequest>()),
            Times.Never);

    }
    
    [Fact]
    public async Task ApprovePasswordResetRequestAsync_ShouldReturnSuccess_WhenRequestIsValid()
    {
        var adminId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var admin = new User
        {
            Id = adminId,
            FullName = "Admin User"
        };
        
        var user = new User
        {
            Id = userId,
            FullName = "Test User",
            Email = "user@email.com"
        };
        
        var request = new PasswordResetRequest
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            User = user,
            ApprovedAt = null
        };

        _passwordResetRequestRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
            .ReturnsAsync(request);
        
        _userRepositoryMock.Setup(r => r.GetByIdAsync(adminId))
            .ReturnsAsync(admin);

        var result = await _authService.ApprovePasswordResetRequestAsync(request.Id, adminId);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.TemporaryPassword.Should().NotBeNullOrEmpty();
        request.ApprovedAt.Should().NotBeNull();
        request.ApprovedById.Should().Be(adminId);
        user.RequiresPasswordChange.Should().BeTrue();
        _passwordResetRequestRepositoryMock.Verify(
            r => r.UpdateAsync(request),
            Times.Once);
        _userRepositoryMock.Verify(
            r => r.UpdateAsync(user),
            Times.Once);
    }
    
    [Fact]
    public async Task ApprovePasswordResetRequestAsync_ShouldReturnFailure_WhenRequestNotFound()
    {
        var requestId = Guid.NewGuid();
        var adminId = Guid.NewGuid();

        _passwordResetRequestRepositoryMock.Setup(r => r.GetByIdAsync(requestId))
            .ReturnsAsync((PasswordResetRequest?)null);

        var result = await _authService.ApprovePasswordResetRequestAsync(requestId, adminId);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Solicitação não encontrada");
    }
    
    [Fact]
    public async Task ApprovePasswordResetRequestAsync_ShouldReturnFailure_WhenRequestAlreadyApproved()
    {
        var adminId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            FullName = "Test User"
        };
        
        var request = new PasswordResetRequest
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            User = user,
            ApprovedAt = DateTime.UtcNow.AddHours(-1),
            ApprovedById = Guid.NewGuid()
        };

        _passwordResetRequestRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
            .ReturnsAsync(request);

        var result = await _authService.ApprovePasswordResetRequestAsync(request.Id, adminId);

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain("Esta solicitação já foi aprovada");
    }
    
    [Fact]
    public async Task ApprovePasswordResetRequestAsync_ShouldGenerateTemporaryPassword()
    {
        var adminId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var admin = new User
        {
            Id = adminId,
            FullName = "Admin User"
        };
        
        var user = new User
        {
            Id = userId,
            FullName = "Test User",
            Email = "user@email.com"
        };
        
        var request = new PasswordResetRequest
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            User = user,
            ApprovedAt = null
        };

        _passwordResetRequestRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
            .ReturnsAsync(request);
        
        _userRepositoryMock.Setup(r => r.GetByIdAsync(adminId))
            .ReturnsAsync(admin);

        var result = await _authService.ApprovePasswordResetRequestAsync(request.Id, adminId);

        result.IsSuccess.Should().BeTrue();
        result.Data!.TemporaryPassword.Should().NotBeNullOrEmpty();
        result.Data.TemporaryPassword.Length.Should().BeGreaterThanOrEqualTo(8);
        request.TemporaryPasswordHash.Should().NotBeNullOrEmpty();
        request.ExpirationDate.Should().NotBeNull();
        request.ExpirationDate.Should().BeAfter(DateTime.UtcNow);
    }
}
