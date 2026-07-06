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
        
        var sql = "SELECT * FROM Asteroids WHERE 1=1 ";
        
        var parameters = new DynamicParameters();

        if (request.OnlyHazardous)
        {
            sql += "AND \"IsPotentiallyHazardous\" = @OnlyHazardous ";
            parameters.Add("OnlyHazardous", request.OnlyHazardous);
        }
        
        sql += "ORDER BY ABS(EXTRACT(EPOCH FROM (\"ClosestApproachDate\" - NOW()))) ASC ";
        
        if (request.Limit.HasValue)
        {
            sql += "LIMIT @Limit ";
            parameters.Add("Limit", request.Limit.Value);
        }
        else if (request.PageSize > 0 && request.Page > 0)
        {
            var offset = (request.Page - 1) * request.PageSize;

            sql += "LIMIT @PageSize OFFSET @Offset ";
            parameters.Add("PageSize", request.PageSize);
            parameters.Add("Offset", offset);
        }

        
        var asteroids = await connection
            .QueryAsync<AsteroidDto>(sql, parameters);
        
        return asteroids;
    }
}