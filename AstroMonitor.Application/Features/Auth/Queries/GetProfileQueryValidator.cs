using FluentValidation;

namespace AstroMonitor.Application.Features.Auth.Queries;

public class GetProfileQueryValidator : AbstractValidator<GetProfileQuery>
{
    public GetProfileQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID required.");
    }
}