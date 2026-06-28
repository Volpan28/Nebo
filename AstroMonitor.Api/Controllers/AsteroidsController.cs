using AstroMonitor.Application.Features.Asteroids.Commands;
using AstroMonitor.Application.Features.Asteroids.Queries.GetAsteroids;
using AstroMonitor.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AstroMonitor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AsteroidsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public AsteroidsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsteroid([FromBody] CreateAsteroidCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAsteroids([FromQuery] int limit = 10)
    {
        var query = new GetAsteroidsQuery(limit);
        var response = await _mediator.Send(query);
        return Ok(response);
    }
}