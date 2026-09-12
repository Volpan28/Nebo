namespace AstroMonitor.Application.Features.Stars.Queries.GetVisible;

public record VisibleStarDto
{
    public string Id { get; init; } = default!;
    public string? ProperName { get; init; }
    public double Magnitude { get; init; }
    public string? ConstellationId { get; init; }
    public double AltitudeDegrees { get; init; } 
}