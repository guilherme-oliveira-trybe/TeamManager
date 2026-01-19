using GFATeamManager.Application.DTOS.Common;
using GFATeamManager.Application.DTOS.Department;
using GFATeamManager.Application.DTOS.Sector;
using GFATeamManager.Application.DTOS.StaffMember;
using GFATeamManager.Application.Services.Interfaces;
using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Interfaces.Repositories;

namespace GFATeamManager.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;

    public DepartmentService(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<BaseResponse<DepartmentDetailResponse>> GetByIdAsync(Guid id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);
        if (department == null)
            return BaseResponse<DepartmentDetailResponse>.Failure("Departamento não encontrado");

        return BaseResponse<DepartmentDetailResponse>.Success(MapToDetailResponse(department));
    }

    public async Task<BaseResponse<IEnumerable<DepartmentDetailResponse>>> GetAllAsync()
    {
        var departments = await _departmentRepository.GetAllWithSectorsAsync();
        var response = departments.Select(MapToDetailResponse).ToList();
        
        return BaseResponse<IEnumerable<DepartmentDetailResponse>>.Success(response);
    }

    public async Task<BaseResponse<DepartmentResponse>> CreateAsync(Guid userId, CreateDepartmentRequest request)
    {
        var department = new Department
        {
            Name = request.Name,
            Description = request.Description
        };

        await _departmentRepository.AddAsync(department);

        return BaseResponse<DepartmentResponse>.Success(MapToResponse(department));
    }

    public async Task<BaseResponse<DepartmentResponse>> UpdateAsync(Guid userId, Guid id, UpdateDepartmentRequest request)
    {
        var department = await _departmentRepository.GetByIdAsync(id);
        if (department == null)
            return BaseResponse<DepartmentResponse>.Failure("Departamento não encontrado");

        department.Name = request.Name;
        department.Description = request.Description;

        await _departmentRepository.UpdateAsync(department);

        return BaseResponse<DepartmentResponse>.Success(MapToResponse(department));
    }

    public async Task<OperationResponse> DeleteAsync(Guid userId, Guid id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);
        if (department == null)
            return OperationResponse.Failure("Departamento não encontrado");

        if (department.Sectors.Any())
            return OperationResponse.Failure("Não é possível deletar departamento com setores ativos");

        await _departmentRepository.DeleteAsync(id);

        return OperationResponse.Success();
    }

    private static DepartmentResponse MapToResponse(Department department)
    {
        return new DepartmentResponse
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description,
            SectorsCount = department.Sectors?.Count ?? 0,
            CreatedAt = department.CreatedAt
        };
    }

    private static DepartmentDetailResponse MapToDetailResponse(Department department)
    {
        return new DepartmentDetailResponse
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description,
            Sectors = department.Sectors?
                .Select(s => new SectorResponse
                {
                    Id = s.Id,
                    DepartmentId = s.DepartmentId,
                    Name = s.Name,
                    Description = s.Description,
                    StaffMembersCount = s.StaffMembers?.Count ?? 0,
                    StaffMembers = s.StaffMembers?
                        .Select(sm => new StaffMemberResponse
                        {
                            Id = sm.Id,
                            SectorId = sm.SectorId,
                            SectorName = s.Name,
                            FullName = sm.FullName,
                            Email = sm.Email,
                            Phone = sm.Phone,
                            Specialty = sm.Specialty,
                            PhotoUrl = sm.PhotoUrl,
                            CreatedAt = sm.CreatedAt
                        })
                        .ToList() ?? [],
                    CreatedAt = s.CreatedAt
                })
                .ToList() ?? [],
            CreatedAt = department.CreatedAt
        };
    }
}
