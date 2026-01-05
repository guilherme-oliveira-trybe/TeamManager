using GFATeamManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GFATeamManager.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Cpf)
            .IsRequired()
            .HasMaxLength(11);
        
        builder.HasIndex(u => u.Cpf)
            .IsUnique();

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.Weight)
            .HasColumnType("decimal(5,2)");

        builder.Property(u => u.Height)
            .IsRequired();

        builder.Property(u => u.Profile)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(u => u.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(u => u.BirthDate)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.HasOne(u => u.EmergencyContact)
            .WithOne(e => e.User)
            .HasForeignKey<EmergencyContact>(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.PreRegistration)
            .WithOne(p => p.User)
            .HasForeignKey<User>(u => u.PreRegistrationId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(u => u.RequiresPasswordChange)
            .IsRequired()
            .HasDefaultValue(false);
    }
}