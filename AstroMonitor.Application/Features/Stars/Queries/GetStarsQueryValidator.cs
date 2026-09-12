using FluentValidation;

namespace AstroMonitor.Application.Features.Stars.Queries;

public class GetStarsQueryValidator : AbstractValidator<GetStarsQuery>
{
    public GetStarsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber must be greater than or equal to 1.");
        
        RuleFor(x => x.PageSize).
            GreaterThanOrEqualTo(25).WithMessage("PageSize must be greater than or equal to 25.")
            .LessThan(101).WithMessage("PageSize must be less than or equal to 101.");
        
        RuleFor(x => x.MaxMagnitude)
            .LessThan(6.5).WithMessage("MaxMagnitude must be less than 6.5.");
    }
}