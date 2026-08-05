namespace AstroMonitor.Application.Features.Watchlist.Commands.Add;

public record CreateWatchlistItemRequest(string ObjectId, string? Note);