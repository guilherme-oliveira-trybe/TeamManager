using GFATeamManager.Application.DTOS.Common;
using GFATeamManager.Application.DTOS.PreRegistration;

namespace GFATeamManager.Application.Services.Interfaces;

public interface IPreRegistrationService
{
    Task<BaseResponse<PreRegistrationResponse>> CreateAsync(CreatePreRegistrationRequest request);
    Task<BaseResponse<PreRegistrationResponse>> GetByIdAsync(Guid id);
    Task<BaseResponse<List<PreRegistrationResponse>>> GetByCpfAsync(string cpf);
    Task<BaseResponse<List<PreRegistrationResponse>>> GetAllAsync();
    Task<OperationResponse> RegenerateCodeAsync(Guid id);
}