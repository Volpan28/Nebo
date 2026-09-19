using AstroMonitor.Application.Features.SkyMap.Queries.Get;
using MediatR;

namespace AstroMonitor.Api.Controllers;
using Microsoft.AspNetCore.Mvc;

[Route("api/skymap")]
public class SkyMapController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public SkyMapController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetSkyMap(
        [FromQuery] double latitude, 
        [FromQuery] double longitude, 
        [FromQuery] DateTime observationDate)
    {
        var query = new GetSkyMapQuery(latitude, longitude, observationDate);
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }
}