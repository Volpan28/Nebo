using MediatR;

namespace AstroMonitor.Application.Features.Auth.Commands.Registration;

public record RegisterUserCommand(
    string FirstName, 
    string LastName, 
    string Email, 
    string Password
    ) : IRequest<TokenResponse>;