using MediatR;

namespace AstroMonitor.Application.Features.Reports.Queries.CsvReportCreate;

public record GenerateCsvReportQuery(
    double Latitude, 
    double Longitude, 
    DateTime ObservationDate) : IRequest<(byte[] FileContents, string FileName)>;