using System.Security.Claims;
using FluentValidation;
using GFATeamManager.Api.Extensions;
using GFATeamManager.Application.DTOS.Auth;
using GFATeamManager.Application.Services.Interfaces;

namespace GFATeamManager.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication");

        group.MapPost("/login", async (
                LoginRequest request,
                IValidator<LoginRequest> validator,
                IAuthService service) =>
            {
                var (isValid, errorResponse) = await validator.ValidateRequest<LoginResponse, LoginRequest>(request);
                if (!isValid)
                    return Results.BadRequest(errorResponse);

                var result = await service.LoginAsync(request);
                return result.IsSuccess
                    ? Results.Ok(result)
                    : Results.Unauthorized();
            })
            .WithName("Login")
            .AllowAnonymous()
            .RequireRateLimiting("login");

        group.MapPost("/change-password", async (
                ChangePasswordRequest request,
                IValidator<ChangePasswordRequest> validator,
                ClaimsPrincipal user,
                IAuthService service) =>
            {
                var (isValid, errorResponse) = await validator.ValidateRequest<object, ChangePasswordRequest>(request);
                if (!isValid)
                    return Results.BadRequest(errorResponse);

                var userId = user.GetUserId();

                var result = await service.ChangePasswordAsync(userId, request);
                return result.IsSuccess
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            })
            .WithName("ChangePassword")
            .RequireAuthorization()
            .RequireRateLimiting("authenticated");
        
        group.MapPost("/request-password-reset", async (
                RequestPasswordResetRequest request,
                IValidator<RequestPasswordResetRequest> validator,
                IAuthService service) =>
            {
                var (isValid, errorResponse) = await validator.ValidateRequest<object, RequestPasswordResetRequest>(request);
                if (!isValid)
                    return Results.BadRequest(errorResponse);
    
                var result = await service.RequestPasswordResetAsync(request);
                return result.IsSuccess 
                    ? Results.Ok(new { message = "Se os dados estiverem corretos, sua solicitação foi enviada ao administrador." })
                    : Results.BadRequest(result);
            })
            .WithName("RequestPasswordReset")
            .AllowAnonymous()
            .RequireRateLimiting("login"); 
        
        group.MapGet("/password-reset-requests/pending", async (
                IAuthService service) =>
            {
                var result = await service.GetPendingPasswordResetRequestsAsync();
                return Results.Ok(result);
            })
            .WithName("GetPendingPasswordResetRequests")
            .RequireAuthorization("AdminOnly")
            .RequireRateLimiting("admin");
        
        group.MapPost("/password-reset-requests/{requestId:guid}/approve", async (
                Guid requestId,
                ClaimsPrincipal user,
                IAuthService service) =>
            {
                var adminId = user.GetUserId();
    
                var result = await service.ApprovePasswordResetRequestAsync(requestId, adminId);
                return result.IsSuccess 
                    ? Results.Ok(result)
                    : Results.BadRequest(result);
            })
            .WithName("ApprovePasswordResetRequest")
            .RequireAuthorization("AdminOnly")
            .RequireRateLimiting("admin");
        
    }
}