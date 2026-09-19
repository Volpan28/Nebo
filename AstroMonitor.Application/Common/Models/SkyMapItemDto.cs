namespace AstroMonitor.Application.Common.Models;

public record SkyMapItemDto(
    string Id,
    string Name,
    string Category,
    double Altitude,
    double Azimuth,
    double Magnitude, 
    string TextureUrl
);