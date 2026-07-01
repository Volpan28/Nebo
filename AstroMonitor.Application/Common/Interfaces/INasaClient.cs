using AstroMonitor.Application.Features.Asteroids.Commands.CreateAsteroids;

namespace AstroMonitor.Application.Common.Interfaces;

public interface INasaClient
{
    Task<IEnumerable<CreateAsteroidCommand>> FetchAsteroidsAsync(
        string startDate, 
        string endDate, 
        CancellationToken cancellationToken = default);
}