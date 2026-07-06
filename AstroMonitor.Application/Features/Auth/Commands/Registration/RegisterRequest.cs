namespace AstroMonitor.Application.Features.Auth.Commands.Registration;

public record RegisterRequest(string FirstName, string LastName, string Email, string Password);