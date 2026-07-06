using AstroMonitor.Application.Features.Auth.Commands.Login;
using AstroMonitor.Application.Features.Auth.Commands.Registration;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = Microsoft.AspNetCore.Identity.Data.LoginRequest;
using RegisterRequest = AstroMonitor.Application.Features.Auth.Commands.Registration.RegisterRequest;

namespace AstroMonitor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IMediator  _mediator;

    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterUserCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password
        );
        
        var tokenResponse = await _mediator.Send(command);
        return Ok(tokenResponse);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginUserCommand(
            request.Email, 
            request.Password
        );
        
        var tokenResponse  = await _mediator.Send(command);
        return Ok(tokenResponse);
    }
}