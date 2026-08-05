using System.Security.Claims;
using AstroMonitor.Application.Features.Watchlist.Commands.Add;
using AstroMonitor.Application.Features.Watchlist.Commands.Remove;
using AstroMonitor.Application.Features.Watchlist.Commands.Update;
using AstroMonitor.Application.Features.Watchlist.Queries.Get;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AstroMonitor.Api.Controllers;


[Authorize]
public class WatchlistController : ApiControllerBase
{
    private readonly IMediator _mediator;
    
    public WatchlistController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserWatchlist()
    {
        var query = new GetUserWatchlistQuery(UserId); // UserId from ApiControllerBase
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddWatchlistItem([FromBody] CreateWatchlistItemRequest request)
    {
        var command = new AddWatchListItemCommand(
            UserId, 
            request.ObjectId,
            request.Note
        );
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{objectId}")]
    public async Task<IActionResult> RemoveWatchlistItem([FromRoute] string objectId)
    {
        var command = new RemoveWatchlistItemCommand(
            UserId, 
            objectId
        );
        
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPatch("{objectId}")]
    public async Task<IActionResult> PatchWatchlistItem([FromRoute] string objectId, [FromBody] UpdateWatchlistNoteRequest request)
    {
        var command = new UpdateWatchlistItemCommand(
            UserId, 
            objectId,
            request.Note
        );
        
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}