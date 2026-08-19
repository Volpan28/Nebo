using System.Globalization;
using AstroMonitor.Application.Common.Interfaces;
using AstroMonitor.Domain.Entities;
using CsvHelper;
using CsvHelper.Configuration;

namespace AstroMonitor.Infrastructure.Files.Parsers;

public class StarCsvParser : IStarCsvParser
{
    public IEnumerable<Star> Parse(Stream csvStream, double maxMagnitude = 6.5)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            HeaderValidated = null
        };

        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, config);

        csv.Context.RegisterClassMap<StarCsvMap>();

        var records = csv.GetRecords<StarCsvDto>();

        foreach (var record in records)
        {
            if (record.Magnitude <= maxMagnitude)
            {
                yield return new Star(
                    record.Id,
                    string.IsNullOrWhiteSpace(record.ProperName) ? null : record.ProperName,
                    record.RightAscension,
                    record.Declination,
                    record.Distance,
                    record.Magnitude,
                    record.ColorIndex,
                    string.IsNullOrWhiteSpace(record.Constellation) ? null : record.Constellation
                );
            }
        }
    }
}