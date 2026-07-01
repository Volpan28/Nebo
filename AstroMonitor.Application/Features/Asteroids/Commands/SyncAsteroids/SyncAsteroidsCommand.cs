using MediatR;

namespace AstroMonitor.Application.Features.Asteroids.Commands.SyncAsteroids;

public record SyncAsteroidsCommand(string StartDate, string EndDate) : IRequest<int>;