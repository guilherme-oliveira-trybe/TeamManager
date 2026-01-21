using GFATeamManager.Application.DTOS.Common;
using GFATeamManager.Application.DTOS.PreRegistration;
using GFATeamManager.Application.Extensions;
using GFATeamManager.Application.Services.Interfaces;
using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Interfaces.Repositories;

namespace GFATeamManager.Application.Services;

public class PreRegistrationService : IPreRegistrationService
{
    private readonly IPreRegistrationRepository _preRegistrationRepository;
    private readonly IUserRepository _userRepository;
    
    public PreRegistrationService(
        IPreRegistrationRepository preRegistrationRepository,
        IUserRepository userRepository)
    {
        _preRegistrationRepository = preRegistrationRepository;
        _userRepository = userRepository;
    }
    
    public async Task<BaseResponse<PreRegistrationResponse>> CreateAsync(CreatePreRegistrationRequest request)
    {
        if (!request.Cpf.IsValidCpf())
            return BaseResponse<PreRegistrationResponse>.Failure("CPF inválido");

        if (await _userRepository.CpfExistsAsync(request.Cpf))
            return BaseResponse<PreRegistrationResponse>.Failure("Já existe um usuário cadastrado com este CPF");

        var preRegistration = new PreRegistration
        {
            Cpf = request.Cpf,
            Profile = request.Profile,
            Unit = request.Unit,
            Position = request.Position
        };

        await _preRegistrationRepository.AddAsync(preRegistration);

        return BaseResponse<PreRegistrationResponse>.Success(MapToResponse(preRegistration));
    }
    
    public async Task<BaseResponse<PreRegistrationResponse>> GetByIdAsync(Guid id)
    {
        var preRegistration = await _preRegistrationRepository.GetByIdAsync(id);
        
        if (preRegistration == null)
            return BaseResponse<PreRegistrationResponse>.Failure("Pré-cadastro não encontrado");

        return BaseResponse<PreRegistrationResponse>.Success(MapToResponse(preRegistration));
    }
    
    public async Task<BaseResponse<List<PreRegistrationResponse>>> GetByCpfAsync(string cpf)
    {
        var cleanCpf = cpf.CleanCpf();
        var preRegistrations = await _preRegistrationRepository.GetUnusedByCpfAsync(cleanCpf);
        
        var response = preRegistrations.Select(MapToResponse).ToList();
        return BaseResponse<List<PreRegistrationResponse>>.Success(response);
    }
    
    public async Task<OperationResponse> RegenerateCodeAsync(Guid id)
    {
        var preRegistration = await _preRegistrationRepository.GetByIdAsync(id);
        
        if (preRegistration == null)
            return OperationResponse.Failure("Pré-cadastro não encontrado");

        if (preRegistration.IsUsed)
            return OperationResponse.Failure("Este pré-cadastro já foi utilizado");

        preRegistration = new PreRegistration
        {
            Cpf = preRegistration.Cpf,
            Profile = preRegistration.Profile,
            Unit = preRegistration.Unit,
            Position = preRegistration.Position
        };

        await _preRegistrationRepository.AddAsync(preRegistration);
        return OperationResponse.Success();
    }
    
    private static PreRegistrationResponse MapToResponse(PreRegistration entity)
    {
        return new PreRegistrationResponse
        {
            Id = entity.Id,
            Cpf = entity.Cpf,
            ActivationCode = entity.ActivationCode,
            Profile = entity.Profile,
            Unit = entity.Unit?.ToString(),
            Position = entity.Position?.ToString(),
            ExpirationDate = entity.ExpirationDate,
            IsUsed = entity.IsUsed,
            UsedAt = entity.UsedAt,
            CreatedAt = entity.CreatedAt
        };
    }
}