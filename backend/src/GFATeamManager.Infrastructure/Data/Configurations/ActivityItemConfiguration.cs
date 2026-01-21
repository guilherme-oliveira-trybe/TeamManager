using GFATeamManager.Domain.Entities;
using GFATeamManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GFATeamManager.Infrastructure.Data.Configurations;

public class ActivityItemConfiguration : IEntityTypeConfiguration<ActivityItem>
{
    public void Configure(EntityTypeBuilder<ActivityItem> builder)
    {
        builder.ToTable("ActivityItems");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(x => x.TargetUnit)
            .HasConversion<int>();



        var positionsConverter = new ValueConverter<List<PlayerPosition>, int[]>(
            v => v.Select(e => (int)e).ToArray(),
            v => v.Select(e => (PlayerPosition)e).ToList());

        var positionsComparer = new ValueComparer<List<PlayerPosition>>(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

        builder.Property(x => x.TargetPositions)
            .HasConversion(positionsConverter)
            .Metadata.SetValueComparer(positionsComparer);
        
        builder.Property(x => x.TargetPositions)
            .HasColumnType("integer[]");
        
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
