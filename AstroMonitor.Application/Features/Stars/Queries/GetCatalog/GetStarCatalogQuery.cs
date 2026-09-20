using MediatR;

namespace AstroMonitor.Application.Features.Stars.Queries.GetCatalog;

public record GetStarCatalogQuery(double? MaxMagnitude) : IRequest<IEnumerable<StarDto>>;
