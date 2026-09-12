using AstroMonitor.Application.Common.Interfaces;
using Dapper;
using MediatR;

namespace AstroMonitor.Application.Features.Stars.Queries.GetVisible;

public class GetVisibleStarsQueryHandler : IRequestHandler<GetVisibleStarsQuery, IEnumerable<VisibleStarDto>>
{
    private readonly ISqlConnectionFactory _sqlConnection;
    private readonly IAstronomyMathService _mathService;

    public GetVisibleStarsQueryHandler(ISqlConnectionFactory sqlConnectionFactory, IAstronomyMathService mathService)
    {
        _sqlConnection = sqlConnectionFactory;
        _mathService = mathService;
    }
    
    public async Task<IEnumerable<VisibleStarDto>> Handle(GetVisibleStarsQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnection.CreateConnection();
        var utcNow = DateTime.UtcNow;

        var sql = "SELECT \"Id\", \"ProperName\", \"Magnitude\", \"ConstellationId\", \"RightAscension\", \"Declination\" FROM \"Stars\"";
        
        var allStars = await connection.QueryAsync<dynamic>(sql);

        var visibleStars = new List<VisibleStarDto>();

        foreach (var star in allStars)
        {
            double ra = (double)star.RightAscension;
            double dec = (double)star.Declination;

            double altitude = _mathService.CalculateAltitude(ra, dec, request.Latitude, request.Longitude, utcNow);

            if (altitude > (request.MinAltitude ?? 0))
            {
                visibleStars.Add(new VisibleStarDto
                {
                    Id = star.Id,
                    ProperName = star.ProperName,
                    Magnitude = (double)star.Magnitude,
                    ConstellationId = star.ConstellationId,
                    AltitudeDegrees = altitude
                });
            }
        }

        return visibleStars.OrderBy(s => s.Magnitude);
    }
}