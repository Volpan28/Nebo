using MediatR;

namespace AstroMonitor.Application.Features.Auth.Queries;

public record GetProfileQuery(string UserId) : IRequest<ProfileDto>;