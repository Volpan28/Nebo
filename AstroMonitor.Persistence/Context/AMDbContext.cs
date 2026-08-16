using AstroMonitor.Application.Common.Interfaces;
using AstroMonitor.Domain.Entities;
using AstroMonitor.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AstroMonitor.Persistence.Context;

public class AMDbContext : IdentityDbContext<ApplicationUser>, IAMDbContext
{
    public AMDbContext(DbContextOptions<AMDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Asteroid> Asteroids => Set<Asteroid>();
    public DbSet<WatchlistItem> WatchlistItems => Set<WatchlistItem>();
    public DbSet<Star> Stars => Set<Star>();
    public DbSet<AstronomicalEvent> AstronomicalEvents => Set<AstronomicalEvent>();
    public DbSet<Constellation> Constellations => Set<Constellation>();
    public DbSet<SolarSystemBody> SolarSystemBodies => Set<SolarSystemBody>();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => base.SaveChangesAsync(cancellationToken);
    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) => base.Database.BeginTransactionAsync(cancellationToken);
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(AMDbContext).Assembly);
        base.OnModelCreating(builder);
        
        builder.Entity<IdentityRole>().HasData
        (
            new IdentityRole
            {
                Id = "6f6e9f29-7be5-496e-9eef-4fa33ede598e",
                Name = "User",
                NormalizedName = "USER"
            },
            new IdentityRole
            {
                Id = "81ecc690-dee7-4dba-806e-8d2b1f7674b2",
                Name = "Admin",
                NormalizedName = "ADMIN"
            }
        );

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var properties = entityType.GetProperties()
                .Where(p => p.ClrType == typeof(DateTimeOffset) 
                            || p.ClrType == typeof(DateTimeOffset?));

            foreach (var property in properties)
            {
                property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTimeOffset, DateTimeOffset>(
                    v => v.ToUniversalTime(),
                    v => v));
            }
        }
    }
}