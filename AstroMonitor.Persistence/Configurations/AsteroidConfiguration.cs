using AstroMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AstroMonitor.Persistence.Configurations;

public class AsteroidConfiguration : IEntityTypeConfiguration<Asteroid>
{
    public void Configure(EntityTypeBuilder<Asteroid> builder)
    {
        builder.ToTable("asteroids");
        
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(a => a.MinDiameterMeters)
            .IsRequired()
            .HasPrecision(10, 2);
        
        builder.Property(a => a.MaxDiameterMeters)
            .IsRequired()
            .HasPrecision(10, 2);

        builder.Property(a => a.ClosestApproachDate)
            .IsRequired();

        builder.Property(a => a.RelativeVelocityKmPerSec)
            .IsRequired()
            .HasPrecision(10, 2);
        
        builder.Property(a => a.IsPotentiallyHazardous)
            .IsRequired();
        
    }
}