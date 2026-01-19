using System.Security.Claims;
using FluentValidation;
using GFATeamManager.Api.Extensions;
using GFATeamManager.Application.DTOS.StaffMember;
using GFATeamManager.Application.Services.Interfaces;

namespace GFATeamManager.Api.Endpoints;

public static class StaffMemberEndpoints
{
    public static void MapStaffMemberEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/staff")
            .WithTags("Staff Members");
        
        group.MapGet("/", async (
            IStaffMemberService staffMemberService) =>
        {
            var result = await staffMemberService.GetAllAsync();
            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
        })
        .WithName("GetAllStaffMembers")
        .RequireAuthorization()
        .RequireRateLimiting("authenticated");

        group.MapGet("/{id:guid}", async (
            Guid id,
            IStaffMemberService staffMemberService) =>
        {
            var result = await staffMemberService.GetByIdAsync(id);
            return result.IsSuccess ? Results.Ok(result) : Results.NotFound(result);
        })
        .WithName("GetStaffMemberById")
        .RequireAuthorization()
        .RequireRateLimiting("authenticated");

        group.MapGet("/sector/{sectorId:guid}", async (
            Guid sectorId,
            IStaffMemberService staffMemberService) =>
        {
            var result = await staffMemberService.GetBySectorIdAsync(sectorId);
            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
        })
        .WithName("GetStaffMembersBySector")
        .RequireAuthorization()
        .RequireRateLimiting("authenticated");

        group.MapPost("/", async (
            CreateStaffMemberRequest request,
            IValidator<CreateStaffMemberRequest> validator,
            IStaffMemberService staffMemberService,
            ClaimsPrincipal user) =>
        {
            var (isValid, errorResponse) = await validator.ValidateRequest<StaffMemberResponse, CreateStaffMemberRequest>(request);
            if (!isValid)
                return Results.BadRequest(errorResponse);
            
            var result = await staffMemberService.CreateAsync(user.GetUserId(), request);

            return result.IsSuccess
                ? Results.Created($"/api/staff/{result.Data!.Id}", result)
                : Results.BadRequest(result);
        })
        .WithName("CreateStaffMember")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("authenticated");

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateStaffMemberRequest request,
            IValidator<UpdateStaffMemberRequest> validator,
            IStaffMemberService staffMemberService,
            ClaimsPrincipal user) =>
        {
            var (isValid, errorResponse) = await validator.ValidateRequest<StaffMemberResponse, UpdateStaffMemberRequest>(request);
            if (!isValid)
                return Results.BadRequest(errorResponse);
            
            var result = await staffMemberService.UpdateAsync(user.GetUserId(), id, request);

            return result.IsSuccess ? Results.Ok(result) : Results.NotFound(result);
        })
        .WithName("UpdateStaffMember")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("authenticated");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IStaffMemberService staffMemberService,
            ClaimsPrincipal user) =>
        {
            var result = await staffMemberService.DeleteAsync(user.GetUserId(), id);

            return result.IsSuccess ? Results.NoContent() : Results.BadRequest(result);
        })
        .WithName("DeleteStaffMember")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("authenticated");
    }
}
