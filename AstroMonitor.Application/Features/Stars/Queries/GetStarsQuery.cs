using AstroMonitor.Domain.Entities;
using MediatR;

namespace AstroMonitor.Application.Features.Stars.Queries;

public record GetStarsQuery(
    int? Page,
    int? PageSize,
    string? ConstellationId,
    double? MaxMagnitude
    ) : IRequest<IEnumerable<StarDto>>;