using AstroMonitor.Domain.Entities;
using AstroMonitor.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AstroMonitor.Persistence.Configurations;

public class SolarSystemBodyConfiguration : IEntityTypeConfiguration<SolarSystemBody>
{
    public void Configure(EntityTypeBuilder<SolarSystemBody> builder)
    {
        builder.ToTable("SolarSystemBody");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(a => a.BodyType)
            .IsRequired()
            .HasConversion(
                a => a.ToString(),
                a => (AstroBodyType)Enum.Parse(typeof(AstroBodyType), a));

        builder.Property(a => a.RadiusKm)
            .IsRequired()
            .HasPrecision(10, 2);
    }
}