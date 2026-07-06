using FluentValidation;

namespace AstroMonitor.Application.Features.Asteroids.Queries.GetAsteroids;

public class GetAsteroidsQueryValidator : AbstractValidator<GetAsteroidsQuery>
{
    public GetAsteroidsQueryValidator()
    {
        RuleFor(x => x.Limit)
            .GreaterThanOrEqualTo(1).WithMessage("Limit must be greater than or equal to 1.")
            .LessThan(25).WithMessage("Limit must be less than or equal to 25.");

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0.");
        
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0.");
    }
}