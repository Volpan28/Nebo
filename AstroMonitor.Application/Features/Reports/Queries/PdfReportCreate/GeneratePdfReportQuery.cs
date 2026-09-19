using MediatR;

namespace AstroMonitor.Application.Features.Reports.Queries.PdfReportCreate;

public record GeneratePdfReportQuery(
    double Latitude, 
    double Longitude, 
    DateTime ObservationDate) : IRequest<(byte[] FileContents, string FileName)>;