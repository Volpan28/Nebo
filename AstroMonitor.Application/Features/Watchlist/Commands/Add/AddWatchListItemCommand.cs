using MediatR;

namespace AstroMonitor.Application.Features.Watchlist.Commands.Add;

public record AddWatchListItemCommand(string UserId, string ObjectId, string Note) : IRequest<string>;