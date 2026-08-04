using AstroMonitor.Application.Common.Interfaces;
using Dapper;
using MediatR;

namespace AstroMonitor.Application.Features.Watchlist.Queries.Get;

public class GetUserWatchlistQueryHandler : IRequestHandler<GetUserWatchlistQuery, IEnumerable<WatchlistItemDto>>
{
    private readonly ISqlConnectionFactory _sqlConnection;

    public GetUserWatchlistQueryHandler(ISqlConnectionFactory sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }
    
    public async Task<IEnumerable<WatchlistItemDto>> Handle(GetUserWatchlistQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnection.CreateConnection();

        var sql = """
                  SELECT "ObjectId", "Note", "CreatedAt" 
                  FROM WatchlistItems 
                  WHERE "UserId" = @UserId
                  """;
        
        var watchlist  = await connection
            .QueryAsync<WatchlistItemDto>(sql, new {UserId = request.UserId});

        return watchlist;
    }
}