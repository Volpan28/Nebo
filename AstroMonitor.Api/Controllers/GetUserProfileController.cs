using System.Security.Claims;
using AstroMonitor.Application.Features.Auth.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AstroMonitor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GetUserProfileController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public GetUserProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetUserProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var query = new GetProfileQuery(userId);
        var result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound("Profile not found.");
        }
        
        return Ok(result);
    }
}