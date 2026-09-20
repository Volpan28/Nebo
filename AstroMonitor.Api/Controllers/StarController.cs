using AstroMonitor.Application.Features.Stars.Queries;
using AstroMonitor.Application.Features.Stars.Queries.GetCatalog;
using AstroMonitor.Application.Features.Stars.Queries.GetVisible;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AstroMonitor.Api.Controllers;

[ApiController]
[Route("api/stars/[controller]")]
public class StarController : ApiControllerBase
{
    private readonly IMediator _mediator;
    
    public StarController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetStars([FromQuery] GetStarsQuery request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    
    [HttpGet("visible")]
    public async Task<IActionResult> GetVisibleStars(
        [FromQuery] double lat,
        [FromQuery] double lon,
        [FromQuery] double? minAltitude)
    {
        var query = new GetVisibleStarsQuery(lat, lon, minAltitude);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("catalog")]
    public async Task<IActionResult> GetStarCatalog([FromQuery] GetStarCatalogQuery request)
    {
        var result = await _mediator.Send(request);
        return Ok(result);
    }
}