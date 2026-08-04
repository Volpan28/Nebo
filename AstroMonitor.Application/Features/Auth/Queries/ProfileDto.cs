namespace AstroMonitor.Application.Features.Auth.Queries;

public record ProfileDto
{
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public DateTime? LastLogin { get; init; } 
}