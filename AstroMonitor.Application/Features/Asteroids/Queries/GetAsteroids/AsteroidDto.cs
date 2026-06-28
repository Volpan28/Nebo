namespace AstroMonitor.Application.Features.Asteroids.Queries.GetAsteroids;

public record AsteroidDto(string Id, string Name, DateTimeOffset ClosestApproachDate, bool IsPotentiallyHazardous);