using AstroMonitor.Application.Features.Asteroids.Commands.CreateAsteroids;
using AstroMonitor.Application.Features.Asteroids.Commands.SyncAsteroids;
using AstroMonitor.Application.Features.Asteroids.Queries.GetAsteroids;
using AstroMonitor.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AstroMonitor.Api.Controllers;

[ApiController]
[Route("api/asteroids/[controller]")]
public class AsteroidsController : ApiControllerBase
{
    private readonly IMediator _mediator;
    
    public AsteroidsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAsteroids([FromQuery] GetAsteroidsQuery request)
    {
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> SyncAsteroids([FromBody] SyncAsteroidsCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(new {SyncedCount = response});
    }
}