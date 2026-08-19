using System.Globalization;
using CsvHelper.Configuration;

namespace AstroMonitor.Infrastructure.Files.Parsers;

public record StarCsvDto
{
    public string Id { get; init; } = default!;
    public string? ProperName { get; init; }
    public double RightAscension { get; init; }
    public double Declination { get; init; }
    public double Distance { get; init; }
    public double Magnitude { get; init; }
    public double ColorIndex { get; init; }
    public string? Constellation { get; init; }
}

public sealed class StarCsvMap : ClassMap<StarCsvDto>
{
    public StarCsvMap()
    {
        Map(m => m.Id).Name("id");
        
        Map(m => m.ProperName).Convert(args => 
        {
            var text = args.Row.GetField("proper");
            return string.IsNullOrWhiteSpace(text) ? string.Empty : text;
        });

        Map(m => m.RightAscension).Name("ra");
        Map(m => m.Declination).Name("dec");
        Map(m => m.Distance).Name("dist");
        Map(m => m.Magnitude).Name("mag");

        Map(m => m.ColorIndex).Convert(args => 
        {
            var text = args.Row.GetField("ci");
            
            if (string.IsNullOrWhiteSpace(text)) 
            {
                return 0.0;
            }
            
            return double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var val) 
                ? val 
                : 0.0;
        });

        Map(m => m.Constellation).Convert(args => 
        {
            var text = args.Row.GetField("con");
            return string.IsNullOrWhiteSpace(text) ? string.Empty : text;
        });
    }
}