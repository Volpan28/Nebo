using FluentValidation;

namespace AstroMonitor.Application.Features.Watchlist.Commands.Add;

public class AddWatchListItemCommandValidator : AbstractValidator<AddWatchListItemCommand>
{
    public AddWatchListItemCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.ObjectId)
            .NotEmpty().WithMessage("ObjectId is required.");

        RuleFor(x => x.Note)
            .MaximumLength(1000).WithMessage("Note cannot exceed 1000 characters.");
    }
}