using AstroMonitor.Application.Common.Models;

namespace AstroMonitor.Application.Common.Interfaces;

public interface IReportGeneratorService
{
    Task<byte[]> GenerateCsvAsync(IEnumerable<VisibilityCsvItemDto> items, CancellationToken cancellationToken);
    Task<byte[]> GeneratePdfAsync(ReportMetadataDto metadata, IEnumerable<VisibilityPdfItemDto> items, CancellationToken cancellationToken);
}