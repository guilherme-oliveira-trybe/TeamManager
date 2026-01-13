using System.Security.Claims;
using FluentValidation;
using GFATeamManager.Api.Extensions;
using GFATeamManager.Application.DTOS.User;
using GFATeamManager.Application.Services.Interfaces;
using GFATeamManager.Domain.Enums;

namespace GFATeamManager.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users");

        group.MapPost("/complete-registration", async (
            CompleteRegistrationRequest request,
            IValidator<CompleteRegistrationRequest> validator,
            IUserService service) =>
        {
            var (isValid, errorResponse) = await validator.ValidateRequest<UserResponse, CompleteRegistrationRequest>(request);
            if (!isValid)
                return Results.BadRequest(errorResponse);
            
            var result = await service.CompleteRegistrationAsync(request);
            return result.IsSuccess 
                ? Results.Created($"/api/users/{result.Data!.Id}", result)
                : Results.BadRequest(result);
        })
        .WithName("CompleteRegistration")
        .AllowAnonymous()
        .RequireRateLimiting("public");

        group.MapGet("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal user,
            IUserService service) => 
        {
            if (!user.CanAccessUser(id))
                return Results.Forbid();
            
            var result = await service.GetByIdAsync(id);
            return result.IsSuccess 
                ? Results.Ok(result)
                : Results.NotFound(result);
        })
        .WithName("GetUserById")
        .RequireAuthorization()
        .RequireRateLimiting("authenticated");

        group.MapGet("/cpf/{cpf}", async (
            string cpf,
            IUserService service) =>
        {
            var result = await service.GetByCpfAsync(cpf);
            return result.IsSuccess 
                ? Results.Ok(result)
                : Results.NotFound(result);
        })
        .WithName("GetUserByCpf")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("authenticated");

        group.MapGet("/status/{status:int}", async (
            int status,
            IUserService service) =>
        {
            if (!Enum.IsDefined(typeof(UserStatus), status))
                return Results.BadRequest("Status inválido");

            var result = await service.GetByStatusAsync((UserStatus)status);
            return Results.Ok(result);
        })
        .WithName("GetUsersByStatus")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("admin");

        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateUserRequest request,
            IValidator<UpdateUserRequest> validator,
            ClaimsPrincipal user,
            IUserService service) =>
        {
            var (isValid, errorResponse) = await validator.ValidateRequest<UserResponse, UpdateUserRequest>(request);
            if (!isValid)
                return Results.BadRequest(errorResponse);
            
            if (!user.CanAccessUser(id))
                return Results.Forbid();
            
            var result = await service.UpdateAsync(id, request);
            return result.IsSuccess 
                ? Results.Ok(result)
                : Results.BadRequest(result);
        })
        .WithName("UpdateUser")
        .RequireAuthorization()
        .RequireRateLimiting("authenticated");

        group.MapPost("/{id:guid}/activate", async (
            Guid id,
            ClaimsPrincipal user,
            IUserService service) =>
        {
            var adminId = user.GetUserId();
            
            var result = await service.ActivateAsync(id, adminId);
            return result.IsSuccess 
                ? Results.Ok(result)
                : Results.BadRequest(result);
        })
        .WithName("ActivateUser")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("admin");

        group.MapPost("/{id:guid}/deactivate", async (
            Guid id,
            IUserService service) =>
        {
            var result = await service.DeactivateAsync(id);
            return result.IsSuccess 
                ? Results.Ok(result)
                : Results.BadRequest(result);
        })
        .WithName("DeactivateUser")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("admin");

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IUserService service) =>
        {
            var result = await service.DeleteAsync(id);
            return result.IsSuccess 
                ? Results.NoContent()
                : Results.BadRequest(result);
        })
        .WithName("DeleteUser")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("admin");

        group.MapPatch("/{id:guid}/position", async (
            Guid id,
            UpdateUserPositionRequest request,
            IUserService service) =>
        {
            var result = await service.UpdatePositionAsync(id, request);
            return result.IsSuccess 
                ? Results.Ok(result)
                : Results.BadRequest(result);
        })
        .WithName("UpdateUserPosition")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("admin");
    }
}