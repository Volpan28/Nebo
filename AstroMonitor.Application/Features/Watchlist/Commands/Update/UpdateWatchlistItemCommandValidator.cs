using FluentValidation;

namespace AstroMonitor.Application.Features.Watchlist.Commands.Update;

public class UpdateWatchlistItemCommandValidator : AbstractValidator<UpdateWatchlistItemCommand>
{
    public UpdateWatchlistItemCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Id is required.");
        
        RuleFor(x => x.ObjectId)
            .NotEmpty().WithMessage("ObjectId is required.");
        
        RuleFor(x => x.Note)
            .MaximumLength(1000).WithMessage("Note cannot exceed 1000 characters.");
    }
}