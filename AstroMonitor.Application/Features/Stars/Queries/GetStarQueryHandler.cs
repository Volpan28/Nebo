using AstroMonitor.Application.Common.Interfaces;
using Dapper;
using MediatR;

namespace AstroMonitor.Application.Features.Stars.Queries;

public class GetStarQueryHandler : IRequestHandler<GetStarsQuery, IEnumerable<StarDto>>
{
    private readonly ISqlConnectionFactory _sqlConnection;

    public GetStarQueryHandler(ISqlConnectionFactory sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }
    
    public async Task<IEnumerable<StarDto>> Handle(GetStarsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnection.CreateConnection();

        var sql = "SELECT * FROM \"Stars\" WHERE 1=1 ";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(request.ConstellationId))
        {
            sql += "AND \"ConstellationId\" = @ConstellationId ";
            parameters.Add("ConstellationId", request.ConstellationId);
        }

        if (request.MaxMagnitude.HasValue)
        {
            sql += "AND \"Magnitude\" <= @MaxMagnitude ";
            parameters.Add("MaxMagnitude", request.MaxMagnitude);
        }

        sql += "ORDER BY \"Magnitude\" ASC ";
        
        if (request.PageSize > 0 && request.Page > 0)
        {
            var offset = (request.Page - 1) * request.PageSize;
            
            sql += "LIMIT @PageSize OFFSET @Offset ";
            parameters.Add("PageSize", request.PageSize);
            parameters.Add("Offset", offset);
        }
        
        var stars = await connection
            .QueryAsync<StarDto>(sql, parameters);
        
        return stars;
    }
}