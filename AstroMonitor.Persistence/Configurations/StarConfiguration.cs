using AstroMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AstroMonitor.Persistence.Configurations;

public class StarConfiguration : IEntityTypeConfiguration<Star>
{
    public void Configure(EntityTypeBuilder<Star> builder)
    {
        builder.ToTable("Stars");
        
        builder.HasKey(s => s.Id);

        builder.HasIndex(s => s.Magnitude);

        builder.Property(s => s.ProperName)
            .HasMaxLength(40);

        builder.Property(s => s.RightAscension)
            .IsRequired()
            .HasPrecision(4, 2);
        
        builder.Property(s => s.Declination)
            .IsRequired()
            .HasPrecision(4, 2);
        
        builder.Property(s => s.Distance)
            .IsRequired()
            .HasPrecision(10, 2);
        
        builder.Property(s => s.Magnitude)
            .IsRequired()
            .HasPrecision(4, 2);
        
        builder.Property(s => s.ColorIndex)
            .HasPrecision(4, 2);
        
        builder.HasOne(s => s.Constellation)
            .WithMany()
            .HasForeignKey(a => a.ConstellationId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}