using MediatR;

namespace AstroMonitor.Application.Features.Stars.Queries.GetVisible;

public record GetVisibleStarsQuery(double Latitude, double Longitude, double? MinAltitude = 0) 
    : IRequest<IEnumerable<VisibleStarDto>>;