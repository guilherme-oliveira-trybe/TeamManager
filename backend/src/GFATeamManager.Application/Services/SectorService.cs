using GFATeamManager.Application.DTOS.Common;
using GFATeamManager.Application.DTOS.Sector;
using GFATeamManager.Application.DTOS.StaffMember;
using GFATeamManager.Application.Services.Interfaces;
using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Interfaces.Repositories;

namespace GFATeamManager.Application.Services;

public class SectorService : ISectorService
{
    private readonly ISectorRepository _sectorRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public SectorService(ISectorRepository sectorRepository, IDepartmentRepository departmentRepository)
    {
        _sectorRepository = sectorRepository;
        _departmentRepository = departmentRepository;
    }

    public async Task<BaseResponse<SectorResponse>> GetByIdAsync(Guid id)
    {
        var sector = await _sectorRepository.GetWithStaffAsync(id);
        if (sector == null)
            return BaseResponse<SectorResponse>.Failure("Setor não encontrado");

        return BaseResponse<SectorResponse>.Success(MapToResponse(sector));
    }

    public async Task<BaseResponse<IEnumerable<SectorResponse>>> GetByDepartmentIdAsync(Guid departmentId)
    {
        var sectors = await _sectorRepository.GetByDepartmentIdAsync(departmentId);
        var response = sectors.Select(MapToResponse).ToList();
        
        return BaseResponse<IEnumerable<SectorResponse>>.Success(response);
    }

    public async Task<BaseResponse<IEnumerable<SectorResponse>>> GetAllAsync()
    {
        var sectors = await _sectorRepository.GetAllWithStaffAsync();
        var response = sectors.Select(MapToResponse).ToList();
        
        return BaseResponse<IEnumerable<SectorResponse>>.Success(response);
    }

    public async Task<BaseResponse<SectorResponse>> CreateAsync(Guid userId, CreateSectorRequest request)
    {
        var departmentExists = await _departmentRepository.ExistsAsync(request.DepartmentId);
        if (!departmentExists)
            return BaseResponse<SectorResponse>.Failure("Departamento não encontrado");

        var sector = new Sector
        {
            DepartmentId = request.DepartmentId,
            Name = request.Name,
            Description = request.Description
        };

        await _sectorRepository.AddAsync(sector);

        return BaseResponse<SectorResponse>.Success(MapToResponse(sector));
    }

    public async Task<BaseResponse<SectorResponse>> UpdateAsync(Guid userId, Guid id, UpdateSectorRequest request)
    {
        var sector = await _sectorRepository.GetByIdAsync(id);
        if (sector == null)
            return BaseResponse<SectorResponse>.Failure("Setor não encontrado");

        sector.Name = request.Name;
        sector.Description = request.Description;

        await _sectorRepository.UpdateAsync(sector);

        return BaseResponse<SectorResponse>.Success(MapToResponse(sector));
    }

    public async Task<OperationResponse> DeleteAsync(Guid userId, Guid id)
    {
        var sector = await _sectorRepository.GetWithStaffAsync(id);
        if (sector == null)
            return OperationResponse.Failure("Setor não encontrado");

        if (sector.StaffMembers.Any())
            return OperationResponse.Failure("Não é possível deletar setor com profissionais ativos");

        await _sectorRepository.DeleteAsync(id);

        return OperationResponse.Success();
    }

    private static SectorResponse MapToResponse(Sector sector)
    {
        return new SectorResponse
        {
            Id = sector.Id,
            DepartmentId = sector.DepartmentId,
            Name = sector.Name,
            Description = sector.Description,
            StaffMembersCount = sector.StaffMembers?.Count ?? 0,
            StaffMembers = sector.StaffMembers?
                .Select(sm => new StaffMemberResponse
                {
                    Id = sm.Id,
                    SectorId = sm.SectorId,
                    SectorName = sector.Name,
                    FullName = sm.FullName,
                    Email = sm.Email,
                    Phone = sm.Phone,
                    Specialty = sm.Specialty,
                    PhotoUrl = sm.PhotoUrl,
                    CreatedAt = sm.CreatedAt
                })
                .ToList() ?? [],
            CreatedAt = sector.CreatedAt
        };
    }
}
