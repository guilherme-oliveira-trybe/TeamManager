using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Enums;
using GFATeamManager.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GFATeamManager.Infrastructure.Data.Seed;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<AppDbContext>();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(nameof(SeedData));

        try
        {
            await SeedMedicalDepartmentAsync(context, logger);
            await SeedRootUserAsync(context, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating root user.");
            throw;
        }
    }

    private static async Task SeedMedicalDepartmentAsync(AppDbContext context, ILogger logger)
    {
        if (await context.Departments.AnyAsync(d => d.Name == "Médico & Performance"))
        {
            logger.LogInformation("Médico & Performance Department already exists. Skipping seed.");
            return;
        }

        var medicalDepartment = new Department
        {
            Id = Guid.NewGuid(),
            Name = "Médico & Performance",
            Description = "Departamento responsável pela saúde e performance dos atletas",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        context.Departments.Add(medicalDepartment);
        await context.SaveChangesAsync();

        logger.LogInformation("Medical & Performance Department created.");

        var sectors = new[]
        {
            new Sector
            {
                Id = Guid.NewGuid(),
                DepartmentId = medicalDepartment.Id,
                Name = "Fisioterapia",
                Description = "Prevenção e tratamento de lesões, reabilitação física",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Sector
            {
                Id = Guid.NewGuid(),
                DepartmentId = medicalDepartment.Id,
                Name = "Massoterapia",
                Description = "Massagens terapêuticas e desportivas",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Sector
            {
                Id = Guid.NewGuid(),
                DepartmentId = medicalDepartment.Id,
                Name = "Nutrição",
                Description = "Planejamento nutricional e acompanhamento alimentar",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Sector
            {
                Id = Guid.NewGuid(),
                DepartmentId = medicalDepartment.Id,
                Name = "Psicologia",
                Description = "Suporte psicológico e preparação mental",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        };

        context.Sectors.AddRange(sectors);
        await context.SaveChangesAsync();

        logger.LogInformation("Created {Count} sectors for Medical & Performance Department.", sectors.Length);
    }

    private static async Task SeedRootUserAsync(AppDbContext context, ILogger logger)
    {
        if (await context.Users.AnyAsync(u => u.Profile == ProfileType.Admin))
        {
            logger.LogInformation("Root user already exists. Skipping seed.");
            return;
        }

        var rootEmail = Environment.GetEnvironmentVariable("ROOT_EMAIL");
        var rootPassword = Environment.GetEnvironmentVariable("ROOT_PASSWORD");
        var rootName = Environment.GetEnvironmentVariable("ROOT_NAME");
        var rootCpf = Environment.GetEnvironmentVariable("ROOT_CPF");
        var rootPhone = Environment.GetEnvironmentVariable("ROOT_PHONE");

        if (string.IsNullOrEmpty(rootEmail) || string.IsNullOrEmpty(rootPassword) || string.IsNullOrEmpty(rootName) || string.IsNullOrEmpty(rootCpf) || string.IsNullOrEmpty(rootPhone))
        {
            logger.LogWarning("ROOT credentials not configured. Skipping root user creation.");
            return;
        }

        var preRegistration = new PreRegistration
        {
            Id = Guid.NewGuid(),
            Cpf = rootCpf,
            Profile = ProfileType.Admin,
            ActivationCode = "ROOT0000",
            ExpirationDate = DateTime.UtcNow.AddYears(100),
            IsUsed = true,
            UsedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        context.PreRegistrations.Add(preRegistration);
        await context.SaveChangesAsync();

        logger.LogInformation("PreRegistration created for root user.");

        var rootUser = new User
        {
            Id = Guid.NewGuid(),
            Cpf = rootCpf,
            FullName = rootName,
            Email = rootEmail,
            Phone = rootPhone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(rootPassword),
            Profile = ProfileType.Admin,
            Status = UserStatus.Active,
            BirthDate = new DateTime(1990, 1, 1),
            Weight = 0,
            Height = 0,
            RequiresPasswordChange = false,
            ActivatedAt = DateTime.UtcNow,
            ActivatedById = null,
            PreRegistrationId = preRegistration.Id,
            PreRegistration = preRegistration,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        context.Users.Add(rootUser);

        preRegistration.UserId = rootUser.Id;
        preRegistration.User = rootUser;

        await context.SaveChangesAsync();

        logger.LogInformation("Root user created successfully with email: {Email}", rootEmail);
    }
}