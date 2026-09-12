using AstroMonitor.Application.Features.Stars.Queries;
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
}