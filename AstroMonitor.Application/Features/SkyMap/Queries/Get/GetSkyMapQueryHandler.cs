using AstroMonitor.Application.Common.Interfaces;
using AstroMonitor.Application.Common.Models;
using AstroMonitor.Domain.Entities;
using AstroMonitor.Domain.Enums;
using Dapper;
using MediatR;

namespace AstroMonitor.Application.Features.SkyMap.Queries.Get;

public class GetSkyMapQueryHandler : IRequestHandler<GetSkyMapQuery, IEnumerable<SkyMapItemDto>>
{
    private readonly ISqlConnectionFactory _sqlConnection;
    private readonly IAstronomyMathService _mathService;

    public GetSkyMapQueryHandler(ISqlConnectionFactory sqlConnection, IAstronomyMathService mathService)
    {
        _sqlConnection = sqlConnection;
        _mathService = mathService;
    }

    public async Task<IEnumerable<SkyMapItemDto>> Handle(GetSkyMapQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnection.CreateConnection();
        var mapItems = new List<SkyMapItemDto>();

        var bodies = (await connection.QueryAsync<SolarSystemBody>("SELECT * FROM \"SolarSystemBody\"")).ToList();
        var earth = bodies.FirstOrDefault(p => p.Id == "earth");

        foreach (var body in bodies.Where(p => p.Id != "earth"))
        {
            double ra, dec;
            
            if (body.Id == "sun")
            {
                (ra, dec) = _mathService.GetSunEquatorial(
                    earth.SemiMajorAxis, earth.Eccentricity, earth.Inclination, earth.MeanAnomaly, earth.ArgumentOfPeriapsis, earth.LongitudeOfAscendingNode, earth.Epoch,
                    request.ObservationDate);
            }
            else 
            {
                (ra, dec) = _mathService.GetEquatorialFromKeplerian(
                    body.SemiMajorAxis, body.Eccentricity, body.Inclination, body.MeanAnomaly, body.ArgumentOfPeriapsis, body.LongitudeOfAscendingNode, body.Epoch,
                    earth.SemiMajorAxis, earth.Eccentricity, earth.Inclination, earth.MeanAnomaly, earth.ArgumentOfPeriapsis, earth.LongitudeOfAscendingNode, earth.Epoch,
                    request.ObservationDate);
            }

            var (alt, az) = _mathService.GetHorizontalCoordinates(ra, dec, request.Latitude, request.Longitude, request.ObservationDate);
            
            if (alt > 0)
            {
                string category = body.BodyType == AstroBodyType.Planet ? "Planet" : "Moon";
                if (body.Id == "sun") category = "Planet"; 

                double magnitude = 0;
                if (body.Id == "sun") magnitude = -26.7;
                else if (body.Id == "moon") magnitude = -12.7;
                else if (category == "Planet") magnitude = -2.0;

                mapItems.Add(new SkyMapItemDto(body.Id, body.Name, category, alt, az, magnitude, body.ImageUrl ?? "default_texture"));
            }
        }

        var dsos = await connection.QueryAsync<dynamic>(
            "SELECT \"Id\", \"Name\", \"Type\", \"RightAscension\", \"Declination\", \"ImageUrl\" FROM \"DeepSkyObjects\"");

        foreach (var dso in dsos)
        {
            var (alt, az) = _mathService.GetHorizontalCoordinates((double)dso.RightAscension, (double)dso.Declination, request.Latitude, request.Longitude, request.ObservationDate);

            if (alt > 0)
            {
                mapItems.Add(new SkyMapItemDto(dso.Id.ToString(), (string)dso.Name, "Deep Sky Object", alt, az, 1.0, (string)dso.ImageUrl ?? "default_nebula"));
            }
        }

        var stars = (await connection.QueryAsync<dynamic>(
            "SELECT \"Id\", \"ProperName\", \"RightAscension\", \"Declination\", \"Magnitude\", \"ImageUrl\", \"ConstellationId\" " +
            "FROM \"Stars\" WHERE \"Magnitude\" <= 3.5 AND \"Magnitude\" > -20")).ToList();

        foreach (var star in stars)
        {
            var (alt, az) = _mathService.GetHorizontalCoordinates((double)star.RightAscension, (double)star.Declination, request.Latitude, request.Longitude, request.ObservationDate);
            
            if (alt > 0)
            {
                string category = star.ConstellationId != null ? $"Star_{star.ConstellationId}" : "Star";
                mapItems.Add(new SkyMapItemDto(
                    star.Id.ToString(), 
                    (string)star.ProperName ?? "Star", 
                    category, alt, az, (double)star.Magnitude, (string)star.ImageUrl ?? "white"));
            }
        }

        var constellations = await connection.QueryAsync<dynamic>("SELECT \"Id\", \"EnglishName\", \"ImageUrl\" FROM \"Constellation\"");

        foreach (var con in constellations)
        {
            string conId = (string)con.Id;
            var conStars = stars.Where(s => (string)s.ConstellationId == conId).ToList();

            if (conStars.Any())
            {
                double avgRa = conStars.Average(s => (double)s.RightAscension);
                double avgDec = conStars.Average(s => (double)s.Declination);

                var (alt, az) = _mathService.GetHorizontalCoordinates(avgRa, avgDec, request.Latitude, request.Longitude, request.ObservationDate);

                if (alt > 0)
                {
                    mapItems.Add(new SkyMapItemDto(
                        conId, 
                        (string)con.EnglishName, 
                        "Constellation", 
                        alt, az, -1.0, (string)con.ImageUrl ?? ""));
                }
            }
        }

        return mapItems;
    }
}