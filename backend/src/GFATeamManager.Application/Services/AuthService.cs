using GFATeamManager.Application.DTOs.Auth;
using GFATeamManager.Application.DTOS.Auth;
using GFATeamManager.Application.DTOS.Common;
using GFATeamManager.Application.Extensions;
using GFATeamManager.Application.Services.Interfaces;
using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Enums;
using GFATeamManager.Domain.Interfaces.Repositories;

namespace GFATeamManager.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordResetRequestRepository _passwordResetRequestRepository;

    public AuthService(IUserRepository userRepository, IJwtService jwtService, IPasswordResetRequestRepository passwordResetRequestRepository)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _passwordResetRequestRepository = passwordResetRequestRepository;
    }

    public async Task<BaseResponse<LoginResponse>> LoginAsync(LoginRequest request)
{
    var isEmail = request.Login.Contains("@");
    
    var user = isEmail
        ? await _userRepository.GetByEmailAsync(request.Login)
        : await _userRepository.GetByCpfAsync(new string(request.Login.Where(char.IsDigit).ToArray()));

    if (user == null)
        return BaseResponse<LoginResponse>.Failure("Login ou senha inválidos");

    var validResetRequest = await _passwordResetRequestRepository.GetValidRequestByUserIdAsync(user.Id);
    
    if (validResetRequest != null)
    {
        if (BCrypt.Net.BCrypt.Verify(request.Password, validResetRequest.TemporaryPasswordHash!))
        {
            validResetRequest.MarkAsUsed();
            await _passwordResetRequestRepository.UpdateAsync(validResetRequest);

            var token = _jwtService.GenerateToken(user);
            var expiresAt = DateTime.UtcNow.AddHours(8);

            return BaseResponse<LoginResponse>.Success(new LoginResponse
            {
                Token = token,
                ExpiresAt = expiresAt,
            });
        }
    }

    if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        return BaseResponse<LoginResponse>.Failure("Login ou senha inválidos");

    if (user.Status != UserStatus.Active)
        return BaseResponse<LoginResponse>.Failure("Usuário não está ativo. Entre em contato com o administrador.");

    var normalToken = _jwtService.GenerateToken(user);
    var normalExpiresAt = DateTime.UtcNow.AddHours(8);

    return BaseResponse<LoginResponse>.Success(new LoginResponse
    {
        Token = normalToken,
        ExpiresAt = normalExpiresAt,
    });
}

    public async Task<OperationResponse> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
    
        if (user == null)
            return OperationResponse.Failure("Usuário não encontrado");

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
        {
            var validResetRequest = await _passwordResetRequestRepository.GetValidRequestByUserIdAsync(userId);
        
            if (validResetRequest == null || 
                !BCrypt.Net.BCrypt.Verify(request.CurrentPassword, validResetRequest.TemporaryPasswordHash!))
            {
                return OperationResponse.Failure("Senha atual incorreta");
            }
        }

        if (request.NewPassword != request.ConfirmNewPassword)
            return OperationResponse.Failure("As senhas não coincidem");

        if (request.NewPassword.Length < 8)
            return OperationResponse.Failure("A nova senha deve ter no mínimo 8 caracteres");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.PasswordChanged();
        await _userRepository.UpdateAsync(user);

        return OperationResponse.Success();
    }

    public async Task<OperationResponse> RequestPasswordResetAsync(RequestPasswordResetRequest request)
    {
        var cleanCpf = request.Cpf.CleanCpf();

        var user = await _userRepository.GetByCpfAsync(cleanCpf);

        if (user == null || user.Email != request.Email)
        {
            return OperationResponse.Success();
        }

        var pendingRequests = await _passwordResetRequestRepository.GetPendingRequestsAsync();
        var hasPending = pendingRequests.Any(r => r.UserId == user.Id);

        if (hasPending)
        {
            return OperationResponse.Failure("Você já tem uma solicitação pendente. Aguarde a aprovação do administrador.");
        }

        var resetRequest = new PasswordResetRequest
        {
            UserId = user.Id
        };

        await _passwordResetRequestRepository.AddAsync(resetRequest);

        return OperationResponse.Success();
    }

    public async Task<BaseResponse<List<PasswordResetRequestResponse>>> GetPendingPasswordResetRequestsAsync()
    {
        var requests = await _passwordResetRequestRepository.GetPendingRequestsAsync();

        var response = requests.Select(r => new PasswordResetRequestResponse
        {
            UserFullName = r.User.FullName,
            ApprovedByName = null,
            TemporaryPassword = null,
            ExpirationDate = null,
            IsUsed = false
        }).ToList();

        return BaseResponse<List<PasswordResetRequestResponse>>.Success(response);
    }

    public async Task<BaseResponse<PasswordResetRequestResponse>> ApprovePasswordResetRequestAsync(Guid requestId, Guid adminId)
    {
        var request = await _passwordResetRequestRepository.GetByIdAsync(requestId);

        if (request == null)
            return BaseResponse<PasswordResetRequestResponse>.Failure("Solicitação não encontrada");

        if (request.ApprovedAt != null)
            return BaseResponse<PasswordResetRequestResponse>.Failure("Esta solicitação já foi aprovada");

        var temporaryPassword = request.Approve(adminId);
        await _passwordResetRequestRepository.UpdateAsync(request);

        request.User.RequirePasswordChange();
        await _userRepository.UpdateAsync(request.User);

        var admin = await _userRepository.GetByIdAsync(adminId);

        var response = new PasswordResetRequestResponse
        {
            UserFullName = request.User.FullName,
            ApprovedByName = admin?.FullName,
            TemporaryPassword = temporaryPassword,
            ExpirationDate = request.ExpirationDate,
            IsUsed = request.IsUsed
        };

        return BaseResponse<PasswordResetRequestResponse>.Success(response);
    }
}