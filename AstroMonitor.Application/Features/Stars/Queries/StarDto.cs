namespace AstroMonitor.Application.Features.Stars.Queries;

public record StarDto {
    public string Id { get; init; }
    public string? ProperName { get; init; }
    public double RightAscension { get; init; }
    public double Declination { get; init; }
    public double Distance { get; init; }
    public double Magnitude { get; init; }
    public double? ColorIndex { get; init; }
}