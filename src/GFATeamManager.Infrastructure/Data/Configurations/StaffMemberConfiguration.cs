using GFATeamManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GFATeamManager.Infrastructure.Data.Configurations;

public class StaffMemberConfiguration : IEntityTypeConfiguration<StaffMember>
{
    public void Configure(EntityTypeBuilder<StaffMember> builder)
    {
        builder.ToTable("StaffMembers");

        builder.HasKey(sm => sm.Id);

        builder.Property(sm => sm.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(sm => sm.Email)
            .HasMaxLength(200);

        builder.Property(sm => sm.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(sm => sm.Specialty)
            .HasMaxLength(100);

        builder.Property(sm => sm.PhotoUrl)
            .HasMaxLength(500);

        builder.Property(sm => sm.SectorId)
            .IsRequired();

        builder.Property(sm => sm.CreatedAt)
            .IsRequired();

        builder.Property(sm => sm.UpdatedAt)
            .IsRequired();

        builder.Property(sm => sm.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(sm => sm.SectorId);
        builder.HasIndex(sm => sm.Email)
            .IsUnique()
            .HasFilter("\"Email\" IS NOT NULL AND \"Email\" != ''");

        builder.HasQueryFilter(sm => !sm.IsDeleted);
    }
}
