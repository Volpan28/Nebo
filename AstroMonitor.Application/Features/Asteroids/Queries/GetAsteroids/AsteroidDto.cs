namespace AstroMonitor.Application.Features.Asteroids.Queries.GetAsteroids;

public record AsteroidDto(
    string Id, 
    string Name, 
    double MinDiameterMeters,
    double MaxDiameterMeters,
    DateTimeOffset ClosestApproachDate,
    double RelativeVelocityKmPerSec,
    bool IsPotentiallyHazardous
    )
{
    public AsteroidDto() : this(
        string.Empty, 
        string.Empty, 
        default, 
        default, 
        default, 
        default, 
        default) { }
}