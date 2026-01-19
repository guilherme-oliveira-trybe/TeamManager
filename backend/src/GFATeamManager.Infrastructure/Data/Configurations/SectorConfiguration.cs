using GFATeamManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GFATeamManager.Infrastructure.Data.Configurations;

public class SectorConfiguration : IEntityTypeConfiguration<Sector>
{
    public void Configure(EntityTypeBuilder<Sector> builder)
    {
        builder.ToTable("Sectors");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Description)
            .HasMaxLength(500);

        builder.Property(s => s.DepartmentId)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .IsRequired();

        builder.Property(s => s.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(s => s.DepartmentId);
        builder.HasIndex(s => s.Name);

        builder.HasQueryFilter(s => !s.IsDeleted);

        builder.HasMany(s => s.StaffMembers)
            .WithOne(sm => sm.Sector)
            .HasForeignKey(sm => sm.SectorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
