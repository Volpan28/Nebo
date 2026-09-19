using AstroMonitor.Application.Common.Models;
using MediatR;

namespace AstroMonitor.Application.Features.SkyMap.Queries.Get;

public record GetSkyMapQuery(
    double Latitude, 
    double Longitude, 
    DateTime ObservationDate) : IRequest<IEnumerable<SkyMapItemDto>>;