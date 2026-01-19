using GFATeamManager.Application.DTOs.Auth;
using GFATeamManager.Application.DTOS.Auth;
using GFATeamManager.Application.DTOS.Common;

namespace GFATeamManager.Application.Services.Interfaces;

public interface IAuthService
{
    Task<BaseResponse<LoginResponse>> LoginAsync(LoginRequest request);
    Task<OperationResponse> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<OperationResponse> RequestPasswordResetAsync(RequestPasswordResetRequest request);
    Task<BaseResponse<List<PasswordResetRequestResponse>>> GetPendingPasswordResetRequestsAsync();
    Task<BaseResponse<PasswordResetRequestResponse>> ApprovePasswordResetRequestAsync(Guid requestId, Guid adminId);
}