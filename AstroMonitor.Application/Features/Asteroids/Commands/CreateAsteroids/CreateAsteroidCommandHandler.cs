using AstroMonitor.Application.Common.Interfaces;
using AstroMonitor.Domain.Entities;
using MediatR;

namespace AstroMonitor.Application.Features.Asteroids.Commands.CreateAsteroids;

public class CreateAsteroidCommandHandler : IRequestHandler<CreateAsteroidCommand, string>
{
    private readonly IAMDbContext _context;

    public CreateAsteroidCommandHandler(IAMDbContext context)
    {
        _context = context;
    }
    
    public async Task<string> Handle(CreateAsteroidCommand request, CancellationToken cancellationToken)
    {
        var asteroid = new Asteroid
        (
            request.Id, 
            request.Name, 
            request.MinDiameterMeters, 
            request.MaxDiameterMeters,
            request.ClosestApproachDate, 
            request.RelativeVelocityKmPerSec, 
            request.IsPotentiallyHazardous
        );

        _context.Asteroids.Add(asteroid);
        await _context.SaveChangesAsync(cancellationToken);
        return asteroid.Id;
    }
}