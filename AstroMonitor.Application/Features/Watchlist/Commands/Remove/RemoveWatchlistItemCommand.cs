using MediatR;

namespace AstroMonitor.Application.Features.Watchlist.Commands.Remove;

public record RemoveWatchlistItemCommand(string UserId, string ObjectId) : IRequest<string>;