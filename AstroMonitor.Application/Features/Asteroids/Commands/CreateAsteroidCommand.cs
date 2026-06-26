using AstroMonitor.Domain.Entities;
using MediatR;

namespace AstroMonitor.Application.Features.Asteroids.Commands;

public record CreateAsteroidCommand (
    string Id, 
    string Name, 
    double MinDiameterMeters, 
    double MaxDiameterMeters,
    DateTimeOffset ClosestApproachDate,
    double RelativeVelocityKmPerSec,
    bool IsPotentiallyHazardous
    ) : IRequest<string>;