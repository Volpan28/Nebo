namespace AstroMonitor.Application.Common.Models;

public record VisibilityCsvItemDto(
    string ObjectName, 
    string ObjectCategory, 
    string Constellation, 
    double AltitudeDegrees, 
    string VisibilityStatus, 
    string? Description);