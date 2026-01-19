using GFATeamManager.Application.DTOS.Activities;
using GFATeamManager.Application.DTOS.Common;
using GFATeamManager.Domain.Enums;

namespace GFATeamManager.Application.Services.Interfaces;

public interface IActivityService
{
    Task<BaseResponse<ActivityResponse>> CreateActivityAsync(Guid userId, CreateActivityRequest request);
    Task<BaseResponse<IEnumerable<ActivityResponse>>> GetActivitiesAsync(Guid userId, ProfileType profile, PlayerUnit? unit, PlayerPosition? position, DateTime startDate, DateTime endDate);
    Task<BaseResponse<ActivityResponse>> GetActivityDetailsAsync(Guid userId, ProfileType profile, PlayerUnit? unit, PlayerPosition? position, Guid activityId);
    Task<BaseResponse<ActivityResponse>> UpdateActivityAsync(Guid userId, Guid activityId, UpdateActivityRequest request);
    Task<OperationResponse> DeleteActivityAsync(Guid userId, Guid activityId);
    
    Task<BaseResponse<ActivityItemResponse>> AddActivityItemAsync(Guid userId, Guid activityId, CreateActivityItemRequest request);
    Task<BaseResponse<ActivityItemResponse>> UpdateActivityItemAsync(Guid userId, Guid activityId, Guid itemId, UpdateActivityItemRequest request);
    Task<OperationResponse> DeleteActivityItemAsync(Guid userId, Guid activityId, Guid itemId);
}
