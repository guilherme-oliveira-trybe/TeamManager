using GFATeamManager.Application.DTOS.Common;
using GFATeamManager.Application.DTOS.Sector;

namespace GFATeamManager.Application.Services.Interfaces;

public interface ISectorService
{
    Task<BaseResponse<SectorResponse>> GetByIdAsync(Guid id);
    Task<BaseResponse<IEnumerable<SectorResponse>>> GetByDepartmentIdAsync(Guid departmentId);
    Task<BaseResponse<IEnumerable<SectorResponse>>> GetAllAsync();
    Task<BaseResponse<SectorResponse>> CreateAsync(Guid userId, CreateSectorRequest request);
    Task<BaseResponse<SectorResponse>> UpdateAsync(Guid userId, Guid id, UpdateSectorRequest request);
    Task<OperationResponse> DeleteAsync(Guid userId, Guid id);
}
