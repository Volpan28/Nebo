using MediatR;

namespace AstroMonitor.Application.Features.Imports.ImportStars.Commands;

public record ImportStarsCommand(Stream stream) : IRequest;