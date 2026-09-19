using System.Reflection.Metadata;
using AstroMonitor.Application.Common.Interfaces;
using AstroMonitor.Application.Common.Models;
using AstroMonitor.Domain.Entities;
using AstroMonitor.Domain.Enums;
using Dapper;
using MediatR;

namespace AstroMonitor.Application.Features.Reports.Queries.PdfReportCreate;

public class GeneratePdfReportQueryHandler : IRequestHandler<GeneratePdfReportQuery, (byte[] FileContents, string FileName)>
{
    private readonly ISqlConnectionFactory _sqlConnection;
    private readonly IAstronomyMathService _mathService;
    private readonly IReportGeneratorService _reportGenerator;

    public GeneratePdfReportQueryHandler(
        ISqlConnectionFactory sqlConnection, 
        IAstronomyMathService mathService,
        IReportGeneratorService reportGenerator)
    {
        _sqlConnection = sqlConnection;
        _mathService = mathService;
        _reportGenerator = reportGenerator;
    }

    public async Task<(byte[] FileContents, string FileName)> Handle(GeneratePdfReportQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnection.CreateConnection();
        var visibleObjects = new List<VisibilityPdfItemDto>();

        var startTime = request.ObservationDate.Date;
        var endTime = startTime.AddHours(6);

        var planets = (await connection.QueryAsync<SolarSystemBody>("SELECT * FROM \"SolarSystemBody\"")).ToList();
        var earth = planets.FirstOrDefault(p => p.Id == "earth");

        foreach (var planet in planets.Where(p => p.Id != "sun" && p.Id != "earth"))
        {
            var (ra, dec) = _mathService.GetEquatorialFromKeplerian(
                planet.SemiMajorAxis, planet.Eccentricity, planet.Inclination, planet.MeanAnomaly, planet.ArgumentOfPeriapsis, planet.LongitudeOfAscendingNode, planet.Epoch,
                earth.SemiMajorAxis, earth.Eccentricity, earth.Inclination, earth.MeanAnomaly, earth.ArgumentOfPeriapsis, earth.LongitudeOfAscendingNode, earth.Epoch,
                startTime); 

            var visibilityTimes = GetVisibilityWindow(ra, dec, request.Latitude, request.Longitude, startTime, endTime);
            if (visibilityTimes != null)
            {
                string category = planet.BodyType == AstroBodyType.Planet ? "Planets" : "Moons";
                visibleObjects.Add(new VisibilityPdfItemDto(planet.Name, category, "Zodiac", visibilityTimes, planet.Description, planet.ImageUrl));
            }
        }

        var dsos = await connection.QueryAsync<dynamic>(
            "SELECT d.\"Name\", d.\"Type\", d.\"RightAscension\", d.\"Declination\", d.\"Description\", d.\"ImageUrl\", c.\"EnglishName\" AS ConstellationName " +
            "FROM \"DeepSkyObjects\" d LEFT JOIN \"Constellation\" c ON d.\"ConstellationId\" = c.\"Id\"");

        foreach (var dso in dsos)
        {
            var visibilityTimes = GetVisibilityWindow((double)dso.RightAscension, (double)dso.Declination, request.Latitude, request.Longitude, startTime, endTime);
            if (visibilityTimes != null)
            {
                visibleObjects.Add(new VisibilityPdfItemDto(dso.Name, "Deep Sky Objects", dso.ConstellationName ?? "-", visibilityTimes, dso.Description, dso.ImageUrl));
            }
        }

        var constellations = await connection.QueryAsync<dynamic>(
            "SELECT \"Id\", \"EnglishName\", \"Description\", \"ImageUrl\" FROM \"Constellation\" WHERE \"Description\" IS NOT NULL");

        foreach (var con in constellations)
        {
            visibleObjects.Add(new VisibilityPdfItemDto((string)con.EnglishName, "Constellations", (string)con.Id, "All Night", (string)con.Description, (string)con.ImageUrl));
        }

        var stars = await connection.QueryAsync<dynamic>(
            "SELECT s.\"ProperName\", s.\"RightAscension\", s.\"Declination\", s.\"Description\", s.\"ImageUrl\", c.\"EnglishName\" AS ConstellationName " +
            "FROM \"Stars\" s " +
            "LEFT JOIN \"Constellation\" c ON s.\"ConstellationId\" = c.\"Id\" " +
            "WHERE s.\"Magnitude\" <= 2.0 AND s.\"ProperName\" IS NOT NULL AND s.\"ProperName\" != ''");

        foreach (var star in stars)
        {
            var visibilityTimes = GetVisibilityWindow((double)star.RightAscension, (double)star.Declination, request.Latitude, request.Longitude, startTime, endTime);
            if (visibilityTimes != null)
            {
                string desc = star.Description ?? $"A bright star in the constellation {star.ConstellationName}. Its celestial coordinates are RA: {Math.Round((double)star.RightAscension, 2)}h, Dec: {Math.Round((double)star.Declination, 2)}°.";
                visibleObjects.Add(new VisibilityPdfItemDto((string)star.ProperName, "Bright Stars", (string)star.ConstellationName ?? "-", visibilityTimes, desc, (string)star.ImageUrl));
            }
        }

        var metadata = new ReportMetadataDto(request.Latitude, request.Longitude, startTime, endTime);
        var fileBytes = await _reportGenerator.GeneratePdfAsync(metadata, visibleObjects, cancellationToken);
        var fileName = $"NightSky_Report_{startTime:yyyyMMdd}.pdf";

        return (fileBytes, fileName);
    }

    private string GetVisibilityWindow(double ra, double dec, double lat, double lon, DateTime start, DateTime end)
    {
        var visibleHours = new List<DateTime>();
        for (var t = start; t <= end; t = t.AddMinutes(30))
        {
            if (_mathService.CalculateAltitude(ra, dec, lat, lon, t) > 0) visibleHours.Add(t);
        }
        if (!visibleHours.Any()) return null;
        return $"{visibleHours.First():HH:mm} - {visibleHours.Last():HH:mm} UTC";
    }
}