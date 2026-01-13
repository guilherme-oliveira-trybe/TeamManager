using GFATeamManager.Application.DTOS.Activities;
using GFATeamManager.Application.DTOS.Common;
using GFATeamManager.Application.Services.Interfaces;
using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Enums;
using GFATeamManager.Domain.Interfaces.Repositories;

namespace GFATeamManager.Application.Services;

public class ActivityService : IActivityService
{
    private readonly IActivityRepository _activityRepository;
    private readonly IUserRepository _userRepository;

    public ActivityService(IActivityRepository activityRepository, IUserRepository userRepository)
    {
        _activityRepository = activityRepository;
        _userRepository = userRepository;
    }

    public async Task<BaseResponse<ActivityResponse>> CreateActivityAsync(Guid userId, CreateActivityRequest request)
    {
        var activity = new Activity
        {
            Type = request.Type,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Location = request.Location,
            TargetUnit = request.TargetUnit,
            CreatedById = userId
        };

        await _activityRepository.AddAsync(activity);

        return BaseResponse<ActivityResponse>.Success(MapToResponse(activity));
    }

    public async Task<BaseResponse<IEnumerable<ActivityResponse>>> GetActivitiesAsync(
        Guid userId, 
        ProfileType profile, 
        PlayerUnit? unit, 
        PlayerPosition? position, 
        DateTime startDate, 
        DateTime endDate)
    {
        var activities = await _activityRepository.GetActivitiesByDateRangeAsync(startDate, endDate);

        if (profile == ProfileType.Admin)
        {
            var allActivities = activities.Select(a => MapToResponse(a)).ToList();
            return BaseResponse<IEnumerable<ActivityResponse>>.Success(allActivities);
        }

        var filteredActivities = activities
            .Where(a => a.TargetUnit == null || a.TargetUnit == unit)
            .Select(a => {
                var visibleItems = FilterItems(a.Items, unit, position);
                return MapToResponse(a, visibleItems);
            })
            .ToList();

        return BaseResponse<IEnumerable<ActivityResponse>>.Success(filteredActivities);
    }

    public async Task<BaseResponse<ActivityResponse>> GetActivityDetailsAsync(
        Guid userId, 
        ProfileType profile, 
        PlayerUnit? unit, 
        PlayerPosition? position, 
        Guid activityId)
    {
        var activity = await _activityRepository.GetByIdAsync(activityId);
        if (activity == null) return BaseResponse<ActivityResponse>.Failure("Activity not found");

        if (profile == ProfileType.Admin)
        {
            return BaseResponse<ActivityResponse>.Success(MapToResponse(activity));
        }

        if (activity.TargetUnit != null && activity.TargetUnit != unit)
            return BaseResponse<ActivityResponse>.Failure("Activity not accessible");

        var visibleItems = FilterItems(activity.Items, unit, position);
        return BaseResponse<ActivityResponse>.Success(MapToResponse(activity, visibleItems));
    }

    public async Task<BaseResponse<ActivityItemResponse>> AddActivityItemAsync(Guid userId, Guid activityId, CreateActivityItemRequest request)
    {
        var activity = await _activityRepository.GetByIdAsync(activityId);
        if (activity == null) return BaseResponse<ActivityItemResponse>.Failure("Activity not found");

        var item = new ActivityItem
        {
            ActivityId = activityId,
            Title = request.Title,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            TargetUnit = request.TargetUnit,
            TargetPositions = request.TargetPositions
        };

        activity.Items.Add(item);
        await _activityRepository.UpdateAsync(activity);
        
        return BaseResponse<ActivityItemResponse>.Success(MapToItemResponse(item));
    }

    public async Task<BaseResponse<ActivityResponse>> UpdateActivityAsync(Guid userId, Guid activityId, UpdateActivityRequest request)
    {
        var activity = await _activityRepository.GetByIdAsync(activityId);
        if (activity == null) return BaseResponse<ActivityResponse>.Failure("Activity not found");

        activity.Type = request.Type;
        activity.StartDate = request.StartDate;
        activity.EndDate = request.EndDate;
        activity.Location = request.Location;
        activity.TargetUnit = request.TargetUnit;

        await _activityRepository.UpdateAsync(activity);
        
        return BaseResponse<ActivityResponse>.Success(MapToResponse(activity));
    }

    public async Task<OperationResponse> DeleteActivityAsync(Guid userId, Guid activityId)
    {
        var activity = await _activityRepository.GetByIdAsync(activityId);
        if (activity == null) return OperationResponse.Failure("Activity not found");

        await _activityRepository.DeleteAsync(activityId);
        
        return OperationResponse.Success();
    }

    public async Task<BaseResponse<ActivityItemResponse>> UpdateActivityItemAsync(Guid userId, Guid activityId, Guid itemId, UpdateActivityItemRequest request)
    {
        var activity = await _activityRepository.GetByIdAsync(activityId);
        if (activity == null) return BaseResponse<ActivityItemResponse>.Failure("Activity not found");

        var item = activity.Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null) return BaseResponse<ActivityItemResponse>.Failure("Activity item not found");

        item.Title = request.Title;
        item.StartTime = request.StartTime;
        item.EndTime = request.EndTime;
        item.TargetUnit = request.TargetUnit;
        item.TargetPositions = request.TargetPositions;

        await _activityRepository.UpdateAsync(activity);
        
        return BaseResponse<ActivityItemResponse>.Success(MapToItemResponse(item));
    }

    public async Task<OperationResponse> DeleteActivityItemAsync(Guid userId, Guid activityId, Guid itemId)
    {
        var activity = await _activityRepository.GetByIdAsync(activityId);
        if (activity == null) return OperationResponse.Failure("Activity not found");

        var item = activity.Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null) return OperationResponse.Failure("Activity item not found");

        activity.Items.Remove(item);
        await _activityRepository.UpdateAsync(activity);
        
        return OperationResponse.Success();
    }

    private static IEnumerable<ActivityItem> FilterItems(IEnumerable<ActivityItem> items, PlayerUnit? unit, PlayerPosition? position)
    {
        return items.Where(i => 
            (i.TargetUnit == null && (i.TargetPositions.Count == 0)) ||
            (i.TargetUnit != null && i.TargetUnit == unit) ||
            (position.HasValue && i.TargetPositions.Contains(position.Value))
        );
    }

    private static ActivityResponse MapToResponse(Activity activity, IEnumerable<ActivityItem>? items = null)
    {
        return new ActivityResponse
        {
            Id = activity.Id,
            Type = activity.Type.ToString(),
            StartDate = activity.StartDate,
            EndDate = activity.EndDate,
            Location = activity.Location,
            TargetUnit = activity.TargetUnit?.ToString(),
            Items = (items ?? activity.Items).Select(MapToItemResponse).ToList()
        };
    }

    private static ActivityItemResponse MapToItemResponse(ActivityItem item)
    {
        return new ActivityItemResponse
        {
            Id = item.Id,
            Title = item.Title,
            StartTime = item.StartTime,
            EndTime = item.EndTime,
            TargetUnit = item.TargetUnit?.ToString(),
            TargetPositions = item.TargetPositions?.Select(p => p.ToString()).ToList() ?? []
        };
    }
}
