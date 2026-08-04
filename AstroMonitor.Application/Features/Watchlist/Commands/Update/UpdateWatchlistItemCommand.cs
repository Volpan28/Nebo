using MediatR;

namespace AstroMonitor.Application.Features.Watchlist.Commands.Update;

public record UpdateWatchlistItemCommand(string UserId, string ObjectId, string Note) : IRequest<string>;