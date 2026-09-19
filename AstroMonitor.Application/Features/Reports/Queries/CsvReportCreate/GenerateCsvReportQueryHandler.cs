using System.Globalization;
using AstroMonitor.Application.Common.Interfaces;
using AstroMonitor.Application.Common.Models;
using AstroMonitor.Domain.Entities;
using Dapper;
using MediatR;

namespace AstroMonitor.Application.Features.Reports.Queries.CsvReportCreate;

public class GenerateCsvReportQueryHandler : IRequestHandler<GenerateCsvReportQuery, (byte[] FileContents, string FileName)>
{
    private readonly ISqlConnectionFactory _sqlConnection;
    private readonly IAstronomyMathService _mathService;
    private readonly IReportGeneratorService _reportGenerator;

    public GenerateCsvReportQueryHandler(
        ISqlConnectionFactory sqlConnection, 
        IAstronomyMathService mathService, 
        IReportGeneratorService reportGenerator)
    {
        _sqlConnection = sqlConnection;
        _mathService = mathService;
        _reportGenerator = reportGenerator;
    }

    public async Task<(byte[] FileContents, string FileName)> Handle(GenerateCsvReportQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnection.CreateConnection();
        var reportItems = new List<VisibilityCsvItemDto>();

        var dsos = await connection.QueryAsync<dynamic>(
            "SELECT d.\"Name\", d.\"RightAscension\", d.\"Declination\", d.\"Description\", c.\"EnglishName\" AS ConstellationName " +
            "FROM \"DeepSkyObjects\" d LEFT JOIN \"Constellation\" c ON d.\"ConstellationId\" = c.\"Id\"");

        foreach (var dso in dsos)
        {
            double alt = _mathService.CalculateAltitude((double)dso.RightAscension, (double)dso.Declination, request.Latitude, request.Longitude, request.ObservationDate);
            if (alt > 0)
            {
                reportItems.Add(new VisibilityCsvItemDto(dso.Name, "Deep Sky Object", dso.ConstellationName ?? "-", Math.Round(alt, 2), "Visible above horizon", dso.Description));
            }
        }

        var planets = (await connection.QueryAsync<SolarSystemBody>("SELECT * FROM \"SolarSystemBody\"")).ToList();
        var earth = planets.FirstOrDefault(p => p.Id == "earth");

        foreach (var planet in planets.Where(p => p.Id != "sun" && p.Id != "earth"))
        {
            var (ra, dec) = _mathService.GetEquatorialFromKeplerian(
                planet.SemiMajorAxis, planet.Eccentricity, planet.Inclination, planet.MeanAnomaly, planet.ArgumentOfPeriapsis, planet.LongitudeOfAscendingNode, planet.Epoch,
                earth.SemiMajorAxis, earth.Eccentricity, earth.Inclination, earth.MeanAnomaly, earth.ArgumentOfPeriapsis, earth.LongitudeOfAscendingNode, earth.Epoch,
                request.ObservationDate);

            double alt = _mathService.CalculateAltitude(ra, dec, request.Latitude, request.Longitude, request.ObservationDate);
            if (alt > 0)
            {
                reportItems.Add(new VisibilityCsvItemDto(planet.Name, planet.BodyType.ToString(), "Moving object", Math.Round(alt, 2), "Visible above horizon", planet.Description));
            }
        }

        var constellations = await connection.QueryAsync<dynamic>(
            "SELECT \"Id\", \"EnglishName\", \"Description\" FROM \"Constellation\" WHERE \"Description\" IS NOT NULL");

        foreach (var con in constellations)
        {
            reportItems.Add(new VisibilityCsvItemDto((string)con.EnglishName, "Constellations", (string)con.Id, 90.0, "All Night", (string)con.Description));
        }

        var stars = await connection.QueryAsync<dynamic>(
            "SELECT s.\"ProperName\", s.\"RightAscension\", s.\"Declination\", s.\"Description\", c.\"EnglishName\" AS ConstellationName " +
            "FROM \"Stars\" s " +
            "LEFT JOIN \"Constellation\" c ON s.\"ConstellationId\" = c.\"Id\" " +
            "WHERE s.\"Magnitude\" <= 2.0 AND s.\"ProperName\" IS NOT NULL AND s.\"ProperName\" != ''");

        foreach (var star in stars)
        {
            double alt = _mathService.CalculateAltitude((double)star.RightAscension, (double)star.Declination, request.Latitude, request.Longitude, request.ObservationDate);
            if (alt > 0)
            {
                string desc = star.Description ?? $"A bright star in the constellation {star.ConstellationName}.";
                reportItems.Add(new VisibilityCsvItemDto((string)star.ProperName, "Bright Stars", (string)star.ConstellationName ?? "-", Math.Round(alt, 2), "Visible above horizon", desc));
            }
        }
        
        var fileBytes = await _reportGenerator.GenerateCsvAsync(reportItems.OrderByDescending(r => r.AltitudeDegrees), cancellationToken);
        var fileName = $"AstroVisibility_{request.ObservationDate:yyyyMMdd_HHmm}.csv";
        
        return (fileBytes, fileName);
    }
}