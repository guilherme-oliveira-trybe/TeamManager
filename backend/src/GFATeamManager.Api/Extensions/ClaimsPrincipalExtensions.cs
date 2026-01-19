using System.Security.Claims;
using GFATeamManager.Domain.Enums;

namespace GFATeamManager.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User ID not found in token"));
    }

    public static string GetUserEmail(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Email)?.Value ?? 
               throw new UnauthorizedAccessException("Email not found in token");
    }

    public static string GetUserRole(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Role)?.Value ?? 
               throw new UnauthorizedAccessException("Role not found in token");
    }
    
    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        return user.GetUserRole() == "Admin";
    }
    
    public static bool IsAthlete(this ClaimsPrincipal user)
    {
        return user.GetUserRole() == "Athlete";
    }
    
    public static bool CanAccessUser(this ClaimsPrincipal user, Guid targetUserId)
    {
        if (user.IsAdmin())
            return true;
    
        return user.GetUserId() == targetUserId;
    }

    public static PlayerUnit? GetUserUnit(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst("unit")?.Value;
        return claim != null ? Enum.Parse<PlayerUnit>(claim) : null;
    }

    public static PlayerPosition? GetUserPosition(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst("position")?.Value;
        return claim != null ? Enum.Parse<PlayerPosition>(claim) : null;
    }
    
    public static ProfileType GetUserProfile(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.Role)?.Value; 
        return claim != null ? Enum.Parse<ProfileType>(claim) : throw new UnauthorizedAccessException("Role not found");
    }
}