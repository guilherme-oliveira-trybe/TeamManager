using GFATeamManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GFATeamManager.Infrastructure.Data.Configurations;

public class PreRegistrationConfiguration : IEntityTypeConfiguration<PreRegistration>
{
    public void Configure(EntityTypeBuilder<PreRegistration> builder)
    {
        builder.ToTable("PreRegistrations");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Cpf)
            .IsRequired()
            .HasMaxLength(11);

        builder.HasIndex(p => p.Cpf);

        builder.Property(p => p.ActivationCode)
            .IsRequired()
            .HasMaxLength(8)
            .IsFixedLength();

        builder.HasIndex(p => p.ActivationCode)
            .IsUnique();

        builder.Property(p => p.Profile)
            .HasConversion<int>()
            .IsRequired();
            
        builder.Property(p => p.Unit)
            .HasConversion<int>();

        builder.Property(p => p.Position)
            .HasConversion<int>();

        builder.Property(p => p.ExpirationDate)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired(false);

        builder.Property(p => p.UsedAt)
            .IsRequired(false);

        builder.Property(p => p.IsUsed)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.UserId)
            .IsRequired(false);

        builder.HasIndex(p => new { p.Cpf, p.IsUsed });
    }
}