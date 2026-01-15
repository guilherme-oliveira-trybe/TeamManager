using GFATeamManager.Application.DTOS.Common;
using GFATeamManager.Application.DTOS.Department;

namespace GFATeamManager.Application.Services.Interfaces;

public interface IDepartmentService
{
    Task<BaseResponse<DepartmentDetailResponse>> GetByIdAsync(Guid id);
    Task<BaseResponse<IEnumerable<DepartmentDetailResponse>>> GetAllAsync();
    Task<BaseResponse<DepartmentResponse>> CreateAsync(Guid userId, CreateDepartmentRequest request);
    Task<BaseResponse<DepartmentResponse>> UpdateAsync(Guid userId, Guid id, UpdateDepartmentRequest request);
    Task<OperationResponse> DeleteAsync(Guid userId, Guid id);
}
