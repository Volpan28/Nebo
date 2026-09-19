using AstroMonitor.Application.Features.Reports.Queries.CsvReportCreate;
using AstroMonitor.Application.Features.Reports.Queries.PdfReportCreate;
using MediatR;

namespace AstroMonitor.Api.Controllers;
using Microsoft.AspNetCore.Mvc;

[Route("api/reports")]
public class ReportController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public ReportController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("csv")]
    public async Task<IActionResult> DownloadCsvReport(
        [FromQuery] double latitude, 
        [FromQuery] double longitude, 
        [FromQuery] DateTime observationDate)
    {
        var query = new GenerateCsvReportQuery(latitude, longitude, observationDate);
        
        var result = await _mediator.Send(query);
        
        return File(result.FileContents, "text/csv", result.FileName);
    }

    [HttpGet("pdf")]
    public async Task<IActionResult> DownloadPdfReport(
        [FromQuery] double latitude, 
        [FromQuery] double longitude, 
        [FromQuery] DateTime observationDate)
    {
        var query = new GeneratePdfReportQuery(latitude, longitude, observationDate);
        
        var result = await _mediator.Send(query);
        
        return File(result.FileContents, "application/pdf", result.FileName);
    }
}