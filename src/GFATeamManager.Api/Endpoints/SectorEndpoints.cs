using System.Security.Claims;
using FluentValidation;
using GFATeamManager.Api.Extensions;
using GFATeamManager.Application.DTOS.Sector;
using GFATeamManager.Application.Services.Interfaces;

namespace GFATeamManager.Api.Endpoints;

public static class SectorEndpoints
{
    public static void MapSectorEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sectors")
            .WithTags("Sectors");
        
        group.MapGet("/", async (
            ISectorService sectorService) =>
        {
            var result = await sectorService.GetAllAsync();
            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
        })
        .WithName("GetAllSectors")
        .RequireAuthorization()
        .RequireRateLimiting("authenticated");

        group.MapGet("/{id:guid}", async (
            Guid id,
            ISectorService sectorService) =>
        {
            var result = await sectorService.GetByIdAsync(id);
            return result.IsSuccess ? Results.Ok(result) : Results.NotFound(result);
        })
        .WithName("GetSectorById")
        .RequireAuthorization()
        .RequireRateLimiting("authenticated");

        group.MapGet("/department/{departmentId:guid}", async (
            Guid departmentId,
            ISectorService sectorService) =>
        {
            var result = await sectorService.GetByDepartmentIdAsync(departmentId);
            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
        })
        .WithName("GetSectorsByDepartment")
        .RequireAuthorization()
        .RequireRateLimiting("authenticated");

        group.MapPost("/", async (
            CreateSectorRequest request,
            IValidator<CreateSectorRequest> validator,
            ISectorService sectorService,
            ClaimsPrincipal user) =>
        {
            var (isValid, errorResponse) = await validator.ValidateRequest<SectorResponse, CreateSectorRequest>(request);
            if (!isValid)
                return Results.BadRequest(errorResponse);
            
            var result = await sectorService.CreateAsync(user.GetUserId(), request);

            return result.IsSuccess
                ? Results.Created($"/api/sectors/{result.Data!.Id}", result)
                : Results.BadRequest(result);
        })
        .WithName("CreateSector")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("authenticated");

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateSectorRequest request,
            IValidator<UpdateSectorRequest> validator,
            ISectorService sectorService,
            ClaimsPrincipal user) =>
        {
            var (isValid, errorResponse) = await validator.ValidateRequest<SectorResponse, UpdateSectorRequest>(request);
            if (!isValid)
                return Results.BadRequest(errorResponse);
            
            var result = await sectorService.UpdateAsync(user.GetUserId(), id, request);

            return result.IsSuccess ? Results.Ok(result) : Results.NotFound(result);
        })
        .WithName("UpdateSector")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("authenticated");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            ISectorService sectorService,
            ClaimsPrincipal user) =>
        {
            var result = await sectorService.DeleteAsync(user.GetUserId(), id);

            return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result);
        })
        .WithName("DeleteSector")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("authenticated");
    }
}
