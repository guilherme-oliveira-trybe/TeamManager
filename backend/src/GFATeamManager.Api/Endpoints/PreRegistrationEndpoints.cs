using FluentValidation;
using GFATeamManager.Api.Extensions;
using GFATeamManager.Application.DTOS.PreRegistration;
using GFATeamManager.Application.Services.Interfaces;

namespace GFATeamManager.Api.Endpoints;

public static class PreRegistrationEndpoints
{
    public static void MapPreRegistrationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/pre-registrations")
            .WithTags("Pre-Registrations");

        group.MapGet("/", async (IPreRegistrationService service) =>
        {
            var result = await service.GetAllAsync();
            return Results.Ok(result);
        })
        .WithName("GetAllPreRegistrations")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("admin");

        group.MapPost("/", async (
            CreatePreRegistrationRequest request,
            IValidator<CreatePreRegistrationRequest> validator,
            IPreRegistrationService service) =>
        {
            var (isValid, errorResponse) = await validator.ValidateRequest<PreRegistrationResponse, CreatePreRegistrationRequest>(request);
            if (!isValid)
                return Results.BadRequest(errorResponse);
            
            var result = await service.CreateAsync(request);
            return result.IsSuccess 
                ? Results.Created($"/api/pre-registrations/{result.Data!.Id}", result)
                : Results.BadRequest(result);
        })
        .WithName("CreatePreRegistration")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("admin");

        group.MapGet("/{id:guid}", async (
            Guid id,
            IPreRegistrationService service) =>
        {
            var result = await service.GetByIdAsync(id);
            return result.IsSuccess 
                ? Results.Ok(result)
                : Results.NotFound(result);
        })
        .WithName("GetPreRegistrationById")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("admin");

        group.MapGet("/cpf/{cpf}", async (
            string cpf,
            IPreRegistrationService service) =>
        {
            var result = await service.GetByCpfAsync(cpf);
            return Results.Ok(result);
        })
        .WithName("GetPreRegistrationsByCpf")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("admin");

        group.MapPost("/{id:guid}/regenerate", async (
            Guid id,
            IPreRegistrationService service) =>
        {
            var result = await service.RegenerateCodeAsync(id);
            return result.IsSuccess 
                ? Results.Ok(result)
                : Results.BadRequest(result);
        })
        .WithName("RegenerateActivationCode")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("admin");
    }
}