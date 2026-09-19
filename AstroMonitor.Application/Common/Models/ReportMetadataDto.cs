namespace AstroMonitor.Application.Common.Models;

public record ReportMetadataDto(
    double Latitude, 
    double Longitude, 
    DateTime StartTime, 
    DateTime EndTime);