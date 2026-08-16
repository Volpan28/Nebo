using AstroMonitor.Domain.Entities;
using AstroMonitor.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AstroMonitor.Persistence.Configurations;

public class AstronomicalEventConfiguration : IEntityTypeConfiguration<AstronomicalEvent>
{
    public void Configure(EntityTypeBuilder<AstronomicalEvent> builder)
    {
        builder.ToTable("AstronomicalEvent");
        
        builder.HasKey(a => a.Id);

        builder.HasIndex(a => a.StartDate);
        
        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(a => a.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(a => a.EventType)
            .IsRequired()
            .HasConversion(
                a => a.ToString(),
                a => (AstroEventType)Enum.Parse(typeof(AstroEventType), a));

        builder.Property(a => a.StartDate)
            .IsRequired();
        
        builder.Property(a => a.EndDate)
            .IsRequired();
        
        builder.Property(a => a.PeakDate)
            .IsRequired();
        
        builder.Property(a => a.IsVisibleNakedEye)
            .IsRequired();
    }
}