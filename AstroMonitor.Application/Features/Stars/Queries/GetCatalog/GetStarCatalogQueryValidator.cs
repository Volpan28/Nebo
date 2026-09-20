using FluentValidation;

namespace AstroMonitor.Application.Features.Stars.Queries.GetCatalog;

public class GetStarCatalogQueryValidator : AbstractValidator<GetStarCatalogQuery>
{
    public GetStarCatalogQueryValidator()
    {
        RuleFor(x => x.MaxMagnitude)
            .LessThanOrEqualTo(6.5).WithMessage("MaxMagnitude must be less than or equal to 6.5.")
            .When(x => x.MaxMagnitude.HasValue);
    }
}
