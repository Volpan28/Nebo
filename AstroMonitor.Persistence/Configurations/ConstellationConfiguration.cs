using AstroMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AstroMonitor.Persistence.Configurations;

public class ConstellationConfiguration : IEntityTypeConfiguration<Constellation>
{
    public void Configure(EntityTypeBuilder<Constellation> builder)
    {
        builder.ToTable("Constellation");
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.LatinName)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(c => c.EnglishName)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(c => c.Family)
            .IsRequired()
            .HasMaxLength(50);
    }
}