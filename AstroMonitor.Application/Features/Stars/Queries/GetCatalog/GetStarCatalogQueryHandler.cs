using AstroMonitor.Application.Common.Interfaces;
using Dapper;
using MediatR;

namespace AstroMonitor.Application.Features.Stars.Queries.GetCatalog;

public class GetStarCatalogQueryHandler : IRequestHandler<GetStarCatalogQuery, IEnumerable<StarDto>>
{
    private readonly ISqlConnectionFactory _sqlConnection;

    public GetStarCatalogQueryHandler(ISqlConnectionFactory sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }

    public async Task<IEnumerable<StarDto>> Handle(GetStarCatalogQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnection.CreateConnection();

        // "Magnitude" > -20 excludes the HYG catalog's id-0 Sun placeholder (RA=0, Dec=0,
        // mag -26.7), which is a fixed catalog entry, not the Sun's real (moving) position.
        const string sql =
            "SELECT * FROM \"Stars\" WHERE \"Magnitude\" <= @MaxMagnitude AND \"Magnitude\" > -20 ORDER BY \"Magnitude\" ASC";

        var parameters = new DynamicParameters();
        parameters.Add("MaxMagnitude", request.MaxMagnitude ?? 6.5);

        var stars = await connection.QueryAsync<StarDto>(sql, parameters);

        return stars;
    }
}
