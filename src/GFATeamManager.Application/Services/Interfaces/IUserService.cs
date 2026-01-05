using GFATeamManager.Application.DTOS.Common;
using GFATeamManager.Application.DTOS.User;
using GFATeamManager.Domain.Enums;

namespace GFATeamManager.Application.Services.Interfaces;

public interface IUserService
{
    Task<BaseResponse<UserResponse>> CompleteRegistrationAsync(CompleteRegistrationRequest request);
    Task<BaseResponse<UserResponse>> GetByIdAsync(Guid id);
    Task<BaseResponse<UserResponse>> GetByCpfAsync(string cpf);
    Task<BaseResponse<List<UserResponse>>> GetByStatusAsync(UserStatus status);
    Task<BaseResponse<UserResponse>> UpdateAsync(Guid id, UpdateUserRequest request);
    Task<OperationResponse> ActivateAsync(Guid userId, Guid adminId);
    Task<OperationResponse> DeactivateAsync(Guid userId);
    Task<OperationResponse> DeleteAsync(Guid userId);
}