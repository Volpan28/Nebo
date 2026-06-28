using AstroMonitor.Application.Common.Interfaces;
using MediatR;
using Dapper;

namespace AstroMonitor.Application.Features.Asteroids.Queries.GetAsteroids;

public class GetAsteroidsQueryHandler : IRequestHandler<GetAsteroidsQuery, IEnumerable<AsteroidDto>>
{
    private readonly ISqlConnectionFactory _sqlConnection;

    public GetAsteroidsQueryHandler(ISqlConnectionFactory sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }
    
    public async Task<IEnumerable<AsteroidDto>> Handle(GetAsteroidsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnection.CreateConnection();
        
        var sql = "SELECT \"Id\", \"Name\", \"ClosestApproachDate\", \"IsPotentiallyHazardous\" " +
                  "FROM asteroids " +
                  "LIMIT @Limit; ";
        
        var asteroids = await connection
            .QueryAsync<AsteroidDto>(sql, new { Limit = request.Limit });
        
        return asteroids;
    }
}