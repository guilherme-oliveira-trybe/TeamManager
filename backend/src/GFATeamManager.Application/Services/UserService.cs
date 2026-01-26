using GFATeamManager.Application.DTOS.Common;
using GFATeamManager.Application.DTOS.User;
using GFATeamManager.Domain.Common.Models;
using GFATeamManager.Application.Services.Interfaces;
using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Enums;
using GFATeamManager.Domain.Interfaces.Repositories;

namespace GFATeamManager.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPreRegistrationRepository _preRegistrationRepository;
    
    public UserService(
        IUserRepository userRepository,
        IPreRegistrationRepository preRegistrationRepository)
    {
        _userRepository = userRepository;
        _preRegistrationRepository = preRegistrationRepository;
    }
    
    public async Task<BaseResponse<UserResponse>> CompleteRegistrationAsync(CompleteRegistrationRequest request)
    {
        var cpf = new string(request.Cpf.Where(char.IsDigit).ToArray());

        var preRegistration = await _preRegistrationRepository
            .GetValidPreRegistrationAsync(cpf, request.ActivationCode.ToUpper());

        if (preRegistration == null)
            return BaseResponse<UserResponse>.Failure("Código de ativação inválido ou expirado");

        if (preRegistration.IsUsed)
            return BaseResponse<UserResponse>.Failure("Este código já foi utilizado");

        if (request.Password != request.ConfirmPassword)
            return BaseResponse<UserResponse>.Failure("As senhas não coincidem");

        if (await _userRepository.EmailExistsAsync(request.Email))
            return BaseResponse<UserResponse>.Failure("Este email já está cadastrado");

        var user = new User
        {
            Cpf = cpf,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Profile = preRegistration.Profile,
            Unit = preRegistration.Unit,
            Position = preRegistration.Position,
            Status = UserStatus.AwaitingActivation,
            FullName = request.FullName,
            BirthDate = request.BirthDate,
            Weight = request.Weight,
            Height = request.Height,
            Phone = request.Phone,
            Email = request.Email,
            PreRegistrationId = preRegistration.Id,
            EmergencyContact = new EmergencyContact
            {
                Name = request.EmergencyContactName,
                Phone = request.EmergencyContactPhone
            }
        };

        await _userRepository.AddAsync(user);

        preRegistration.IsUsed = true;
        preRegistration.UsedAt = DateTime.UtcNow;
        preRegistration.UserId = user.Id;
        await _preRegistrationRepository.UpdateAsync(preRegistration);

        return BaseResponse<UserResponse>.Success(MapToResponse(user));
    }
    
    public async Task<BaseResponse<UserResponse>> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
            return BaseResponse<UserResponse>.Failure("Usuário não encontrado");

        return BaseResponse<UserResponse>.Success(MapToResponse(user));
    }
    
    public async Task<BaseResponse<UserResponse>> GetByCpfAsync(string cpf)
    {
        var cleanCpf = new string(cpf.Where(char.IsDigit).ToArray());
        var user = await _userRepository.GetByCpfAsync(cleanCpf);
        
        if (user == null)
            return BaseResponse<UserResponse>.Failure("Usuário não encontrado");

        return BaseResponse<UserResponse>.Success(MapToResponse(user));
    }
    
    public async Task<BaseResponse<List<UserResponse>>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        var response = users.Select(MapToResponse).ToList();
        
        return BaseResponse<List<UserResponse>>.Success(response);
    }

    public async Task<BaseResponse<PagedList<UserResponse>>> GetAllPagedAsync(UserParameters parameters)
    {
        var pagedUsers = await _userRepository.GetAllPagedAsync(
            parameters.PageNumber, 
            parameters.PageSize, 
            parameters.SearchTerm, 
            parameters.Status.HasValue ? (UserStatus)parameters.Status.Value : null
        );
        
        var responseItems = pagedUsers.Select(MapToResponse).ToList();
        
        var pagedResponse = new PagedList<UserResponse>(
            responseItems, 
            pagedUsers.TotalCount, 
            pagedUsers.CurrentPage, 
            pagedUsers.PageSize);
            
        return BaseResponse<PagedList<UserResponse>>.Success(pagedResponse);
    }
    
    public async Task<BaseResponse<List<UserResponse>>> GetByStatusAsync(UserStatus status)
    {
        var users = await _userRepository.GetByStatusAsync(status);
        var response = users.Select(MapToResponse).ToList();
        
        return BaseResponse<List<UserResponse>>.Success(response);
    }
    
    public async Task<BaseResponse<UserResponse>> UpdateAsync(Guid id, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
            return BaseResponse<UserResponse>.Failure("Usuário não encontrado");

        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null && existingUser.Id != id)
            return BaseResponse<UserResponse>.Failure("Este email já está cadastrado");

        user.FullName = request.FullName;
        user.BirthDate = request.BirthDate;
        user.Weight = request.Weight;
        user.Height = request.Height;
        user.Phone = request.Phone;
        user.Email = request.Email;

        if (user.EmergencyContact != null && 
            !string.IsNullOrEmpty(request.EmergencyContactName))
        {
            user.EmergencyContact.Name = request.EmergencyContactName;
            user.EmergencyContact.Phone = request.EmergencyContactPhone ?? string.Empty;
        }

        await _userRepository.UpdateAsync(user);

        return BaseResponse<UserResponse>.Success(MapToResponse(user));
    }
    
    public async Task<OperationResponse> ActivateAsync(Guid userId, Guid adminId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
            return OperationResponse.Failure("Usuário não encontrado");

        if (user.Status != UserStatus.AwaitingActivation)
            return OperationResponse.Failure("Usuário não está aguardando ativação");

        user.Activate(adminId);
        await _userRepository.UpdateAsync(user);

        return OperationResponse.Success();
    }
    
    public async Task<OperationResponse> DeactivateAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
            return OperationResponse.Failure("Usuário não encontrado");

        user.Deactivate();
        await _userRepository.UpdateAsync(user);

        return OperationResponse.Success();
    }
    
    public async Task<OperationResponse> DeleteAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
            return OperationResponse.Failure("Usuário não encontrado");

        await _userRepository.DeleteAsync(userId);

        return OperationResponse.Success();
    }

    public async Task<BaseResponse<UserResponse>> UpdatePositionAsync(Guid id, UpdateUserPositionRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
            return BaseResponse<UserResponse>.Failure("Usuário não encontrado");

        user.Unit = request.Unit;
        user.Position = request.Position;

        await _userRepository.UpdateAsync(user);

        return BaseResponse<UserResponse>.Success(MapToResponse(user));
    }
    
    internal static UserResponse MapToResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Cpf = user.Cpf,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            BirthDate = user.BirthDate,
            Weight = user.Weight,
            Height = user.Height,
            Profile = user.Profile,
            Status = user.Status,
            Unit = user.Unit?.ToString(),
            Position = user.Position?.ToString(),
            CreatedAt = user.CreatedAt,
            ActivatedAt = user.ActivatedAt,
            EmergencyContact = user.EmergencyContact != null 
                ? new EmergencyContactResponse
                {
                    Name = user.EmergencyContact.Name,
                    Phone = user.EmergencyContact.Phone
                }
                : null,
            RequiresPasswordChange = user.RequiresPasswordChange
        };
    }
}