using MediatR;

namespace AstroMonitor.Application.Features.Asteroids.Queries.GetAsteroids;

public record GetAsteroidsQuery(int Limit = 10) : IRequest<IEnumerable<AsteroidDto>>;