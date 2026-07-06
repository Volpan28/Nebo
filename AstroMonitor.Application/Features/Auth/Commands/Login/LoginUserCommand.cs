using MediatR;

namespace AstroMonitor.Application.Features.Auth.Commands.Login;

public record LoginUserCommand(
    string Email, 
    string Password
    ) : IRequest<TokenResponse>;