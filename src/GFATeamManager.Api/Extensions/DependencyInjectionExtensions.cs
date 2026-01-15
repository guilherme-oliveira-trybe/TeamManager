using GFATeamManager.Application.Services;
using GFATeamManager.Application.Services.Interfaces;
using GFATeamManager.Domain.Interfaces.Repositories;
using GFATeamManager.Infrastructure.Data.Repositories;

namespace GFATeamManager.Api.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPreRegistrationRepository, PreRegistrationRepository>();
        services.AddScoped<IPasswordResetRequestRepository, PasswordResetRequestRepository>();
        services.AddScoped<IActivityRepository, ActivityRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<ISectorRepository, SectorRepository>();
        services.AddScoped<IStaffMemberRepository, StaffMemberRepository>();
        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPreRegistrationService, PreRegistrationService>();
        services.AddScoped<IActivityService, ActivityService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<ISectorService, SectorService>();
        services.AddScoped<IStaffMemberService, StaffMemberService>();
        return services;
    }
}