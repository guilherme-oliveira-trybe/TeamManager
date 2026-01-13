using GFATeamManager.Application.Services;
using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Enums;
using GFATeamManager.Domain.Interfaces.Repositories;
using Moq;

namespace GFATeamManager.Application.Tests.Services;

public class ActivityServiceTests
{
    private readonly Mock<IActivityRepository> _activityRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly ActivityService _sut;

    public ActivityServiceTests()
    {
        _activityRepositoryMock = new Mock<IActivityRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _sut = new ActivityService(_activityRepositoryMock.Object, _userRepositoryMock.Object);
    }

    [Fact]
    public async Task GetActivitiesAsync_ShouldReturnOnlyVisibleActivitiesAndItems()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Unit = PlayerUnit.Offense, Position = PlayerPosition.QB, Profile = ProfileType.Athlete };
        
        // _userRepositoryMock setup not needed as we pass claims directly

        var activities = new List<Activity>
        {
            // 1. Global Activity (Visible), with mixed items
            new Activity 
            { 
                Id = Guid.NewGuid(), 
                TargetUnit = null, // Visible
                Items = new List<ActivityItem>
                {
                    new ActivityItem { Id = Guid.NewGuid(), Title = "Global", TargetUnit = null, TargetPositions = new() }, // Visible
                    new ActivityItem { Id = Guid.NewGuid(), Title = "Offense", TargetUnit = PlayerUnit.Offense }, // Visible
                    new ActivityItem { Id = Guid.NewGuid(), Title = "Defense", TargetUnit = PlayerUnit.Defense }, // Hidden
                    new ActivityItem { Id = Guid.NewGuid(), Title = "QB", TargetPositions = new() { PlayerPosition.QB } }, // Visible
                    new ActivityItem { Id = Guid.NewGuid(), Title = "WR", TargetPositions = new() { PlayerPosition.WR } }  // Hidden
                }
            },
            // 2. Offense Activity (Visible)
            new Activity { Id = Guid.NewGuid(), TargetUnit = PlayerUnit.Offense, Items = new List<ActivityItem>() },
            // 3. Defense Activity (Hidden)
            new Activity { Id = Guid.NewGuid(), TargetUnit = PlayerUnit.Defense, Items = new List<ActivityItem>() }
        };

        _activityRepositoryMock.Setup(x => x.GetActivitiesByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(activities);

        // Act
        var result = await _sut.GetActivitiesAsync(userId, user.Profile, user.Unit, user.Position, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data!.Count()); // Activity 1 and 2

        var activity1 = result.Data!.First(a => a.TargetUnit == null);
        Assert.Equal(3, activity1.Items.Count); 
        
        Assert.Contains(activity1.Items, i => i.Title == "Global");
        Assert.Contains(activity1.Items, i => i.Title == "Offense");
        Assert.Contains(activity1.Items, i => i.Title == "QB");
        
        Assert.DoesNotContain(activity1.Items, i => i.Title == "Defense");
        Assert.DoesNotContain(activity1.Items, i => i.Title == "WR");
    }
    [Fact]
    public async Task GetActivitiesAsync_ShouldReturnAllActivitiesForAdmin()
    {
        // Arrange
        var userId = Guid.NewGuid();
        // Admin with no Unit/Position targeting
        var admin = new User { Id = userId, Profile = ProfileType.Admin, Unit = null, Position = null };
        
        // _userRepositoryMock setup not needed

        var activities = new List<Activity>
        {
            new Activity 
            { 
                Id = Guid.NewGuid(), 
                TargetUnit = PlayerUnit.Offense, 
                Items = new List<ActivityItem>
                {
                    new ActivityItem { TargetUnit = PlayerUnit.Offense }
                }
            },
            new Activity 
            { 
                Id = Guid.NewGuid(), 
                TargetUnit = PlayerUnit.Defense, // Normally hidden for admin if treated as "no unit"
                Items = new List<ActivityItem>()
            }
        };

        _activityRepositoryMock.Setup(x => x.GetActivitiesByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(activities);

        // Act
        var result = await _sut.GetActivitiesAsync(userId, admin.Profile, admin.Unit, admin.Position, DateTime.UtcNow, DateTime.UtcNow);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data!.Count()); // Should see both
    }

    [Fact]
    public async Task UpdateActivityAsync_ShouldUpdateActivitySuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var activityId = Guid.NewGuid();
        var activity = new Activity 
        { 
            Id = activityId, 
            Type = ActivityType.Training,
            Location = "Old Location",
            Items = new List<ActivityItem>()
        };

        _activityRepositoryMock.Setup(x => x.GetByIdAsync(activityId)).ReturnsAsync(activity);
        _activityRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Activity>())).ReturnsAsync(activity);

        var request = new Application.DTOS.Activities.UpdateActivityRequest
        {
            Type = ActivityType.Game,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddHours(2),
            Location = "New Location",
            TargetUnit = PlayerUnit.Defense
        };

        // Act
        var result = await _sut.UpdateActivityAsync(userId, activityId, request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(ActivityType.Game, activity.Type);
        Assert.Equal("New Location", activity.Location);
        Assert.Equal(PlayerUnit.Defense, activity.TargetUnit);
        _activityRepositoryMock.Verify(x => x.UpdateAsync(activity), Times.Once);
    }

    [Fact]
    public async Task DeleteActivityAsync_ShouldDeleteActivitySuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var activityId = Guid.NewGuid();
        var activity = new Activity { Id = activityId, Items = new List<ActivityItem>() };

        _activityRepositoryMock.Setup(x => x.GetByIdAsync(activityId)).ReturnsAsync(activity);
        _activityRepositoryMock.Setup(x => x.DeleteAsync(activityId)).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.DeleteActivityAsync(userId, activityId);

        // Assert
        Assert.True(result.IsSuccess);
        _activityRepositoryMock.Verify(x => x.DeleteAsync(activityId), Times.Once);
    }

    [Fact]
    public async Task UpdateActivityItemAsync_ShouldUpdateItemSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var activityId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        
        var item = new ActivityItem 
        { 
            Id = itemId,
            Title = "Old Title",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1)
        };
        
        var activity = new Activity 
        { 
            Id = activityId,
            Items = new List<ActivityItem> { item }
        };

        _activityRepositoryMock.Setup(x => x.GetByIdAsync(activityId)).ReturnsAsync(activity);
        _activityRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Activity>())).ReturnsAsync(activity);

        var request = new Application.DTOS.Activities.UpdateActivityItemRequest
        {
            Title = "New Title",
            StartTime = DateTime.UtcNow.AddHours(2),
            EndTime = DateTime.UtcNow.AddHours(3),
            TargetUnit = PlayerUnit.Offense,
            TargetPositions = new List<PlayerPosition> { PlayerPosition.QB }
        };

        // Act
        var result = await _sut.UpdateActivityItemAsync(userId, activityId, itemId, request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("New Title", item.Title);
        Assert.Equal(PlayerUnit.Offense, item.TargetUnit);
        Assert.Single(item.TargetPositions);
        Assert.Contains(PlayerPosition.QB, item.TargetPositions);
        _activityRepositoryMock.Verify(x => x.UpdateAsync(activity), Times.Once);
    }

    [Fact]
    public async Task DeleteActivityItemAsync_ShouldDeleteItemSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var activityId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        
        var item = new ActivityItem { Id = itemId };
        var activity = new Activity 
        { 
            Id = activityId,
            Items = new List<ActivityItem> { item }
        };

        _activityRepositoryMock.Setup(x => x.GetByIdAsync(activityId)).ReturnsAsync(activity);
        _activityRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Activity>())).ReturnsAsync(activity);

        // Act
        var result = await _sut.DeleteActivityItemAsync(userId, activityId, itemId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(activity.Items);
        _activityRepositoryMock.Verify(x => x.UpdateAsync(activity), Times.Once);
    }

    [Fact]
    public async Task CreateActivityAsync_ShouldCreateActivitySuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new Application.DTOS.Activities.CreateActivityRequest
        {
            Type = ActivityType.Training,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddHours(2),
            Location = "Test Location",
            TargetUnit = PlayerUnit.Offense
        };

        Activity capturedActivity = null!;
        _activityRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Activity>()))
            .Callback<Activity>(a => capturedActivity = a)
            .ReturnsAsync((Activity a) => a);

        // Act
        var result = await _sut.CreateActivityAsync(userId, request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(capturedActivity);
        Assert.Equal(ActivityType.Training, capturedActivity.Type);
        Assert.Equal("Test Location", capturedActivity.Location);
        Assert.Equal(userId, capturedActivity.CreatedById);
        _activityRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Activity>()), Times.Once);
    }

    [Fact]
    public async Task GetActivityDetailsAsync_ShouldReturnActivityForAdmin()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var activityId = Guid.NewGuid();
        var activity = new Activity 
        { 
            Id = activityId,
            TargetUnit = PlayerUnit.Defense,
            Items = new List<ActivityItem>
            {
                new ActivityItem { TargetUnit = PlayerUnit.Defense }
            }
        };

        _activityRepositoryMock.Setup(x => x.GetByIdAsync(activityId)).ReturnsAsync(activity);

        // Act
        var result = await _sut.GetActivityDetailsAsync(userId, ProfileType.Admin, null, null, activityId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Data!.Items); // Admin sees all items
    }

    [Fact]
    public async Task GetActivityDetailsAsync_ShouldFilterItemsForAthlete()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var activityId = Guid.NewGuid();
        var activity = new Activity 
        { 
            Id = activityId,
            TargetUnit = null, // Global activity
            Items = new List<ActivityItem>
            {
                new ActivityItem { Title = "Offense", TargetUnit = PlayerUnit.Offense },
                new ActivityItem { Title = "Defense", TargetUnit = PlayerUnit.Defense }
            }
        };

        _activityRepositoryMock.Setup(x => x.GetByIdAsync(activityId)).ReturnsAsync(activity);

        // Act
        var result = await _sut.GetActivityDetailsAsync(userId, ProfileType.Athlete, PlayerUnit.Offense, PlayerPosition.QB, activityId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Data!.Items);
        Assert.Equal("Offense", result.Data.Items.First().Title);
    }

    [Fact]
    public async Task GetActivityDetailsAsync_ShouldReturnNotFound_WhenActivityDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var activityId = Guid.NewGuid();

        _activityRepositoryMock.Setup(x => x.GetByIdAsync(activityId)).ReturnsAsync((Activity?)null);

        // Act
        var result = await _sut.GetActivityDetailsAsync(userId, ProfileType.Athlete, PlayerUnit.Offense, PlayerPosition.QB, activityId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Activity not found", result.Errors);
    }

    [Fact]
    public async Task GetActivityDetailsAsync_ShouldReturnNotAccessible_WhenUnitDoesNotMatch()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var activityId = Guid.NewGuid();
        var activity = new Activity 
        { 
            Id = activityId,
            TargetUnit = PlayerUnit.Defense,
            Items = new List<ActivityItem>()
        };

        _activityRepositoryMock.Setup(x => x.GetByIdAsync(activityId)).ReturnsAsync(activity);

        // Act
        var result = await _sut.GetActivityDetailsAsync(userId, ProfileType.Athlete, PlayerUnit.Offense, PlayerPosition.QB, activityId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Activity not accessible", result.Errors);
    }

    [Fact]
    public async Task AddActivityItemAsync_ShouldAddItemSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var activityId = Guid.NewGuid();
        var activity = new Activity 
        { 
            Id = activityId,
            Items = new List<ActivityItem>()
        };

        _activityRepositoryMock.Setup(x => x.GetByIdAsync(activityId)).ReturnsAsync(activity);
        _activityRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Activity>())).ReturnsAsync(activity);

        var request = new Application.DTOS.Activities.CreateActivityItemRequest
        {
            Title = "New Item",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1),
            TargetUnit = PlayerUnit.Offense,
            TargetPositions = new List<PlayerPosition> { PlayerPosition.QB }
        };

        // Act
        var result = await _sut.AddActivityItemAsync(userId, activityId, request);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(activity.Items);
        Assert.Equal("New Item", activity.Items.First().Title);
        _activityRepositoryMock.Verify(x => x.UpdateAsync(activity), Times.Once);
    }

    [Fact]
    public async Task AddActivityItemAsync_ShouldReturnNotFound_WhenActivityDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var activityId = Guid.NewGuid();

        _activityRepositoryMock.Setup(x => x.GetByIdAsync(activityId)).ReturnsAsync((Activity?)null);

        var request = new Application.DTOS.Activities.CreateActivityItemRequest
        {
            Title = "Test",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1)
        };

        // Act
        var result = await _sut.AddActivityItemAsync(userId, activityId, request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Activity not found", result.Errors);
    }

    [Fact]
    public async Task UpdateActivityAsync_ShouldReturnNotFound_WhenActivityDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var activityId = Guid.NewGuid();

        _activityRepositoryMock.Setup(x => x.GetByIdAsync(activityId)).ReturnsAsync((Activity?)null);

        var request = new Application.DTOS.Activities.UpdateActivityRequest
        {
            Type = ActivityType.Game,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddHours(2),
            Location = "Test"
        };

        // Act
        var result = await _sut.UpdateActivityAsync(userId, activityId, request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Activity not found", result.Errors);
    }

    [Fact]
    public async Task DeleteActivityAsync_ShouldReturnNotFound_WhenActivityDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var activityId = Guid.NewGuid();

        _activityRepositoryMock.Setup(x => x.GetByIdAsync(activityId)).ReturnsAsync((Activity?)null);

        // Act
        var result = await _sut.DeleteActivityAsync(userId, activityId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Activity not found", result.Errors);
    }

    [Fact]
    public async Task UpdateActivityItemAsync_ShouldReturnNotFound_WhenActivityDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var activityId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        _activityRepositoryMock.Setup(x => x.GetByIdAsync(activityId)).ReturnsAsync((Activity?)null);

        var request = new Application.DTOS.Activities.UpdateActivityItemRequest
        {
            Title = "Test",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1)
        };

        // Act
        var result = await _sut.UpdateActivityItemAsync(userId, activityId, itemId, request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Activity not found", result.Errors);
    }

    [Fact]
    public async Task UpdateActivityItemAsync_ShouldReturnNotFound_WhenItemDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var activityId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var activity = new Activity 
        { 
            Id = activityId,
            Items = new List<ActivityItem>() // Empty list
        };

        _activityRepositoryMock.Setup(x => x.GetByIdAsync(activityId)).ReturnsAsync(activity);

        var request = new Application.DTOS.Activities.UpdateActivityItemRequest
        {
            Title = "Test",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1)
        };

        // Act
        var result = await _sut.UpdateActivityItemAsync(userId, activityId, itemId, request);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Activity item not found", result.Errors);
    }

    [Fact]
    public async Task DeleteActivityItemAsync_ShouldReturnNotFound_WhenActivityDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var activityId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        _activityRepositoryMock.Setup(x => x.GetByIdAsync(activityId)).ReturnsAsync((Activity?)null);

        // Act
        var result = await _sut.DeleteActivityItemAsync(userId, activityId, itemId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Activity not found", result.Errors);
    }

    [Fact]
    public async Task DeleteActivityItemAsync_ShouldReturnNotFound_WhenItemDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var activityId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var activity = new Activity 
        { 
            Id = activityId,
            Items = new List<ActivityItem>() // Empty list
        };

        _activityRepositoryMock.Setup(x => x.GetByIdAsync(activityId)).ReturnsAsync(activity);

        // Act
        var result = await _sut.DeleteActivityItemAsync(userId, activityId, itemId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Activity item not found", result.Errors);
    }
}
