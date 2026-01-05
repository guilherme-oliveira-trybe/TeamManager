namespace GFATeamManager.Domain.Enums;

public enum UserStatus
{
    PendingRegistration = 1,
    AwaitingActivation = 2,
    Active = 3,
    Rejected = 4,
    Inactive = 5
}