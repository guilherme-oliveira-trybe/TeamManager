using GFATeamManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GFATeamManager.Infrastructure.Data.Configurations;

public class PasswordResetRequestConfiguration : IEntityTypeConfiguration<PasswordResetRequest>
{
    public void Configure(EntityTypeBuilder<PasswordResetRequest> builder)
    {
        builder.ToTable("PasswordResetRequests");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.TemporaryPasswordHash)
            .HasMaxLength(500);

        builder.Property(p => p.IsUsed)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.ApprovedBy)
            .WithMany()
            .HasForeignKey(p => p.ApprovedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => new { p.UserId, p.ApprovedAt, p.IsUsed });
    }
}