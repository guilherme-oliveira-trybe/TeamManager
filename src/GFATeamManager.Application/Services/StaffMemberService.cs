using GFATeamManager.Application.DTOS.Common;
using GFATeamManager.Application.DTOS.StaffMember;
using GFATeamManager.Application.Services.Interfaces;
using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Interfaces.Repositories;

namespace GFATeamManager.Application.Services;

public class StaffMemberService : IStaffMemberService
{
    private readonly IStaffMemberRepository _staffMemberRepository;
    private readonly ISectorRepository _sectorRepository;

    public StaffMemberService(IStaffMemberRepository staffMemberRepository, ISectorRepository sectorRepository)
    {
        _staffMemberRepository = staffMemberRepository;
        _sectorRepository = sectorRepository;
    }

    public async Task<BaseResponse<StaffMemberResponse>> GetByIdAsync(Guid id)
    {
        var staffMember = await _staffMemberRepository.GetByIdAsync(id);
        if (staffMember == null)
            return BaseResponse<StaffMemberResponse>.Failure("Profissional não encontrado");

        return BaseResponse<StaffMemberResponse>.Success(MapToResponse(staffMember));
    }

    public async Task<BaseResponse<IEnumerable<StaffMemberResponse>>> GetBySectorIdAsync(Guid sectorId)
    {
        var staffMembers = await _staffMemberRepository.GetBySectorIdAsync(sectorId);
        var response = staffMembers.Select(MapToResponse).ToList();
        
        return BaseResponse<IEnumerable<StaffMemberResponse>>.Success(response);
    }

    public async Task<BaseResponse<IEnumerable<StaffMemberResponse>>> GetAllAsync()
    {
        var staffMembers = await _staffMemberRepository.GetAllAsync();
        var response = staffMembers.Select(MapToResponse).ToList();
        
        return BaseResponse<IEnumerable<StaffMemberResponse>>.Success(response);
    }

    public async Task<BaseResponse<StaffMemberResponse>> CreateAsync(Guid userId, CreateStaffMemberRequest request)
    {
        var sectorExists = await _sectorRepository.ExistsAsync(request.SectorId);
        if (!sectorExists)
            return BaseResponse<StaffMemberResponse>.Failure("Setor não encontrado");

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var emailExists = await _staffMemberRepository.EmailExistsAsync(request.Email);
            if (emailExists)
                return BaseResponse<StaffMemberResponse>.Failure("Email já cadastrado");
        }

        var staffMember = new StaffMember
        {
            SectorId = request.SectorId,
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            Specialty = request.Specialty
        };

        await _staffMemberRepository.AddAsync(staffMember);

        return BaseResponse<StaffMemberResponse>.Success(MapToResponse(staffMember));
    }

    public async Task<BaseResponse<StaffMemberResponse>> UpdateAsync(Guid userId, Guid id, UpdateStaffMemberRequest request)
    {
        var staffMember = await _staffMemberRepository.GetByIdAsync(id);
        if (staffMember == null)
            return BaseResponse<StaffMemberResponse>.Failure("Profissional não encontrado");

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var emailExists = await _staffMemberRepository.EmailExistsAsync(request.Email, id);
            if (emailExists)
                return BaseResponse<StaffMemberResponse>.Failure("Email já cadastrado");
        }

        staffMember.FullName = request.FullName;
        staffMember.Email = request.Email;
        staffMember.Phone = request.Phone;
        staffMember.Specialty = request.Specialty;

        await _staffMemberRepository.UpdateAsync(staffMember);

        return BaseResponse<StaffMemberResponse>.Success(MapToResponse(staffMember));
    }

    public async Task<OperationResponse> DeleteAsync(Guid userId, Guid id)
    {
        var staffMember = await _staffMemberRepository.GetByIdAsync(id);
        if (staffMember == null)
            return OperationResponse.Failure("Profissional não encontrado");

        await _staffMemberRepository.DeleteAsync(id);

        return OperationResponse.Success();
    }

    private static StaffMemberResponse MapToResponse(StaffMember staffMember)
    {
        return new StaffMemberResponse
        {
            Id = staffMember.Id,
            SectorId = staffMember.SectorId,
            SectorName = staffMember.Sector?.Name ?? string.Empty,
            FullName = staffMember.FullName,
            Email = staffMember.Email,
            Phone = staffMember.Phone,
            Specialty = staffMember.Specialty,
            PhotoUrl = staffMember.PhotoUrl,
            CreatedAt = staffMember.CreatedAt
        };
    }
}
