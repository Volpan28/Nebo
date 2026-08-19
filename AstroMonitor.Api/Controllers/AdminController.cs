using AstroMonitor.Application.Features.Imports.ImportStars.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AstroMonitor.Api.Controllers;

[ApiController]
[Route("api/admin/[controller]")]
public class AdminController : ApiControllerBase
{
    private readonly IMediator _mediator;
    
    public  AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("import")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> ImportStars([FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is empty");
        }

        using var stream = file.OpenReadStream();

        var command = new ImportStarsCommand(stream);
        await _mediator.Send(command, cancellationToken);
        return Ok(new { message = "Import completed successfully." });
    }
}