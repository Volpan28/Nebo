using MediatR;

namespace AstroMonitor.Application.Features.Asteroids.Queries.GetAsteroids;

public record GetAsteroidsQuery(
    int? Limit,
    int? Page,
    int? PageSize,
    bool OnlyHazardous
    ) : IRequest<IEnumerable<AsteroidDto>>;