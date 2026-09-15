using AstroMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AstroMonitor.Persistence.Configurations;

public class DeepSkyObjectConfiguration : IEntityTypeConfiguration<DeepSkyObject>
{
    public void Configure(EntityTypeBuilder<DeepSkyObject> builder)
    {
        builder.HasKey(d => d.Id);
        
        builder.Property(d => d.Name).IsRequired().HasMaxLength(150);
        builder.Property(d => d.CatalogName).HasMaxLength(50);
        builder.Property(d => d.Description).IsRequired(); 
        builder.Property(d => d.ImageUrl).HasMaxLength(500);

        builder.HasOne(d => d.Constellation)
            .WithMany()
            .HasForeignKey(d => d.ConstellationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}