using FluentValidation;

namespace AstroMonitor.Application.Features.Watchlist.Commands.Remove;

public class RemoveWatchlistItemCommandValidator : AbstractValidator<RemoveWatchlistItemCommand>
{
    public RemoveWatchlistItemCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("A user ID is required for deletion.");
        
        RuleFor(x => x.ObjectId)
            .NotEmpty().WithMessage("A object ID is required for deletion.");
    }
}