using System.Security.Claims;
using FluentValidation;
using GFATeamManager.Api.Extensions;
using GFATeamManager.Application.DTOS.Activities;
using GFATeamManager.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GFATeamManager.Api.Endpoints;

public static class ActivityEndpoints
{
    public static void MapActivityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/activities")
            .WithTags("Activities");
        
        group.MapPost("/", async (
            CreateActivityRequest request,
            IValidator<CreateActivityRequest> validator,
            IActivityService activityService,
            ClaimsPrincipal user) =>
        {
            var (isValid, errorResponse) = await validator.ValidateRequest<ActivityResponse, CreateActivityRequest>(request);
            if (!isValid)
                return Results.BadRequest(errorResponse);
            
            var result = await activityService.CreateActivityAsync(user.GetUserId(), request);

            return result.IsSuccess
                ? Results.Created($"/api/activities/{result.Data!.Id}", result)
                : Results.BadRequest(result);
        })
        .WithName("CreateActivity")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("authenticated");

        group.MapGet("/", async (
            IActivityService activityService,
            ClaimsPrincipal user,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate) =>
        {
            var now = DateTime.UtcNow;
            
            var weekStartBrazil = now.GetWeekStartInBrazil();
            var weekEndBrazil = now.GetWeekEndInBrazil();
            
            var weekStartUtc = weekStartBrazil.ToUtc();
            var weekEndUtc = weekEndBrazil.ToUtc();
            
            var start = startDate?.NormalizeDateFromQuery() ?? weekStartUtc;
            var end = endDate?.NormalizeDateFromQuery() ?? weekEndUtc;

            var result = await activityService.GetActivitiesAsync(
                user.GetUserId(),
                user.GetUserProfile(),
                user.GetUserUnit(),
                user.GetUserPosition(),
                start, end);
            
            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
        })
        .WithName("GetActivities")
        .RequireAuthorization()
        .RequireRateLimiting("authenticated");
        
        group.MapGet("/{id:guid}", async (
            Guid id,
            IActivityService activityService,
            ClaimsPrincipal user) =>
        {
            var result = await activityService.GetActivityDetailsAsync(
                user.GetUserId(),
                user.GetUserProfile(),
                user.GetUserUnit(),
                user.GetUserPosition(),
                id);
            
            return result.IsSuccess ? Results.Ok(result) : Results.NotFound(result);
        })
        .WithName("GetActivityDetails")
        .RequireAuthorization()
        .RequireRateLimiting("authenticated");
        
        group.MapPut("/{id:guid}", async (
            Guid id,
            UpdateActivityRequest request,
            IValidator<UpdateActivityRequest> validator,
            IActivityService activityService,
            ClaimsPrincipal user) =>
        {
            var (isValid, errorResponse) = await validator.ValidateRequest<ActivityResponse, UpdateActivityRequest>(request);
            if (!isValid)
                return Results.BadRequest(errorResponse);
            
            var result = await activityService.UpdateActivityAsync(user.GetUserId(), id, request);
            
            return result.IsSuccess ? Results.Ok(result) : Results.NotFound(result);
        })
        .WithName("UpdateActivity")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("authenticated");
        
        group.MapDelete("/{id:guid}", async (
            Guid id,
            IActivityService activityService,
            ClaimsPrincipal user) =>
        {
            var result = await activityService.DeleteActivityAsync(user.GetUserId(), id);
            
            return result.IsSuccess ? Results.NoContent() : Results.NotFound(result);
        })
        .WithName("DeleteActivity")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("authenticated");
        
        group.MapPost("/{id:guid}/items", async (
            Guid id,
            CreateActivityItemRequest request,
            IValidator<CreateActivityItemRequest> validator,
            IActivityService activityService,
            ClaimsPrincipal user) =>
        {
            var (isValid, errorResponse) = await validator.ValidateRequest<ActivityItemResponse, CreateActivityItemRequest>(request);
            if (!isValid)
                return Results.BadRequest(errorResponse);
            
            var result = await activityService.AddActivityItemAsync(user.GetUserId(), id, request);
            
            return result.IsSuccess 
                ? Results.Created($"/api/activities/{id}/items/{result.Data!.Id}", result) 
                : Results.NotFound(result);
        })
        .WithName("AddActivityItem")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("authenticated");
        
        group.MapPut("/{activityId:guid}/items/{itemId:guid}", async (
            Guid activityId,
            Guid itemId,
            UpdateActivityItemRequest request,
            IValidator<UpdateActivityItemRequest> validator,
            IActivityService activityService,
            ClaimsPrincipal user) =>
        {
            var (isValid, errorResponse) = await validator.ValidateRequest<ActivityItemResponse, UpdateActivityItemRequest>(request);
            if (!isValid)
                return Results.BadRequest(errorResponse);
            
            var result = await activityService.UpdateActivityItemAsync(user.GetUserId(), activityId, itemId, request);
            
            return result.IsSuccess ? Results.Ok(result) : Results.NotFound(result);
        })
        .WithName("UpdateActivityItem")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("authenticated");
        
        group.MapDelete("/{activityId:guid}/items/{itemId:guid}", async (
            Guid activityId,
            Guid itemId,
            IActivityService activityService,
            ClaimsPrincipal user) =>
        {
            var result = await activityService.DeleteActivityItemAsync(user.GetUserId(), activityId, itemId);
            
            return result.IsSuccess ? Results.NoContent() : Results.NotFound(result);
        })
        .WithName("DeleteActivityItem")
        .RequireAuthorization("AdminOnly")
        .RequireRateLimiting("authenticated");
    }
}
