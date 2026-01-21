using GFATeamManager.Application.DTOS.Common;
using GFATeamManager.Application.DTOS.StaffMember;

namespace GFATeamManager.Application.Services.Interfaces;

public interface IStaffMemberService
{
    Task<BaseResponse<StaffMemberResponse>> GetByIdAsync(Guid id);
    Task<BaseResponse<IEnumerable<StaffMemberResponse>>> GetBySectorIdAsync(Guid sectorId);
    Task<BaseResponse<IEnumerable<StaffMemberResponse>>> GetAllAsync();
    Task<BaseResponse<StaffMemberResponse>> CreateAsync(Guid userId, CreateStaffMemberRequest request);
    Task<BaseResponse<StaffMemberResponse>> UpdateAsync(Guid userId, Guid id, UpdateStaffMemberRequest request);
    Task<OperationResponse> DeleteAsync(Guid userId, Guid id);
}
