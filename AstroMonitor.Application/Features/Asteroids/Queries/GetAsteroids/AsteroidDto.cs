namespace AstroMonitor.Application.Features.Asteroids.Queries.GetAsteroids;

public record AsteroidDto(string Id, string Name, DateTimeOffset ClosestApproachDate, bool IsPotentiallyHazardous)
{
    public AsteroidDto() : this(string.Empty, string.Empty, default, default) { }
}