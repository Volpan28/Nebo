using AstroMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AstroMonitor.Application.Common.Interfaces;

public interface IAMDbContext
{
    DbSet<Asteroid> Asteroids { get; }
    DbSet<WatchlistItem> WatchlistItems { get; }
    DbSet<Star> Stars { get; }
    DbSet<AstronomicalEvent> AstronomicalEvents { get; }
    DbSet<Constellation> Constellations { get; }
    DbSet<SolarSystemBody> SolarSystemBodies { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}