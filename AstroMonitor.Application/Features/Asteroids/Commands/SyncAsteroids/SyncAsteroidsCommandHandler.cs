using AstroMonitor.Application.Common.Interfaces;
using AstroMonitor.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AstroMonitor.Application.Features.Asteroids.Commands.SyncAsteroids;

public class SyncAsteroidsCommandHandler : IRequestHandler<SyncAsteroidsCommand, int>
{
    private readonly INasaClient _client;
    private readonly IAMDbContext _context;

    public SyncAsteroidsCommandHandler(INasaClient client, IAMDbContext context)
    {
        _client = client;
        _context = context;
    }


    public async Task<int> Handle(SyncAsteroidsCommand request, CancellationToken cancellationToken)
    {
        var command = await _client.FetchAsteroidsAsync(request.StartDate, request.EndDate, cancellationToken);

        if (command == null || !command.Any())
        {
            return 0;
        }
        
        var nasaAsteroidsIds = command.Select(c => c.Id).ToList();
        
        var existingAsteroidIds = await _context.Asteroids
            .Where(a => nasaAsteroidsIds.Contains(a.Id))
            .Select(a => a.Id)
            .ToListAsync(cancellationToken);

        var newAsteroids  = command
            .Where(cmd => !existingAsteroidIds.Contains(cmd.Id))
            .Select(cmd => new Asteroid(
                cmd.Id,
                cmd.Name,
                cmd.MinDiameterMeters,
                cmd.MaxDiameterMeters,
                cmd.ClosestApproachDate,
                cmd.RelativeVelocityKmPerSec,
                cmd.IsPotentiallyHazardous
        )).ToList();

        if (!newAsteroids.Any())
        {
            return 0;
        }
        
        _context.Asteroids.AddRange(newAsteroids);
        await _context.SaveChangesAsync(cancellationToken);
        
        return newAsteroids.Count;
    }
}