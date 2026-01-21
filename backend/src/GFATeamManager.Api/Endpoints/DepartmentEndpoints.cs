using System.Security.Claims;
using FluentValidation;
using GFATeamManager.Api.Extensions;
using GFATeamManager.Application.DTOS.Department;
using GFATeamManager.Application.Services.Interfaces;

namespace GFATeamManager.Api.Endpoints;

public static class DepartmentEndpoints
{
    public static void MapDepartmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/departments")
            .WithTags("Departments");
        
        group.MapGet("/", async (
            IDepartmentService departmentService) =>
        {
            var result = await departmentService.GetAllAsync();
            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
        })
        .WithName("GetAllDepartments")
        .RequireAuthorization()
        .RequireRateLimiting("authenticated");

        group.MapGet("/{id:guid}", async (
            Guid id,
            IDepartmentService departmentService) =>
        {
            var result = await departmentService.GetByIdAsync(id);
            return result.IsSuccess ? Results.Ok(result) : Results.NotFound(result);
        })
        .WithName("GetDepartmentById")
        .RequireAuthorization()
        .RequireRateLimiting("authenticated");

        group.MapPost("/", async (
            CreateDepartmentRequest request,
            IValidator<CreateDepartmentRequest> validator,
            IDepartmentService departmentService,
            ClaimsPrincipal user) =>
        {
            var (isValid, errorResponse) = await validator.ValidateRequest<DepartmentResponse, CreateDepartmentRequest>(request);
            if (!isValid)
                return Results.BadRequest(errorResponse);
            
            var result = await departmentService.CreateAsync(user.GetUserId(), request);

            return result.IsSuccess
                ? Results.Created($"/api/departments/{result.Data!.Id}", result)
                : Results.BadRequest(result);
        })
        .WithName("CreateDepartment")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("authenticated");

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateDepartmentRequest request,
            IValidator<UpdateDepartmentRequest> validator,
            IDepartmentService departmentService,
            ClaimsPrincipal user) =>
        {
            var (isValid, errorResponse) = await validator.ValidateRequest<DepartmentResponse, UpdateDepartmentRequest>(request);
            if (!isValid)
                return Results.BadRequest(errorResponse);
            
            var result = await departmentService.UpdateAsync(user.GetUserId(), id, request);

            return result.IsSuccess ? Results.Ok(result) : Results.NotFound(result);
        })
        .WithName("UpdateDepartment")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("authenticated");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IDepartmentService departmentService,
            ClaimsPrincipal user) =>
        {
            var result = await departmentService.DeleteAsync(user.GetUserId(), id);

            return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result);
        })
        .WithName("DeleteDepartment")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("authenticated");
    }
}
