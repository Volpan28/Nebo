using MediatR;

namespace AstroMonitor.Application.Features.Watchlist.Queries.Get;

public record GetUserWatchlistQuery(string UserId) : IRequest<IEnumerable<WatchlistItemDto>>;