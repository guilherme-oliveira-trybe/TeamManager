using Microsoft.EntityFrameworkCore;
using GFATeamManager.Domain.Entities;

namespace GFATeamManager.Infrastructure.Data.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users => Set<User>();
    public DbSet<PreRegistration> PreRegistrations => Set<PreRegistration>();
    public DbSet<EmergencyContact> EmergencyContacts => Set<EmergencyContact>();
    public DbSet<PasswordResetRequest> PasswordResetRequests => Set<PasswordResetRequest>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<ActivityItem> ActivityItems => Set<ActivityItem>();
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", false);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        
        modelBuilder.Entity<User>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<PreRegistration>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<EmergencyContact>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<PasswordResetRequest>().HasQueryFilter(r => !r.IsDeleted);
    }
}