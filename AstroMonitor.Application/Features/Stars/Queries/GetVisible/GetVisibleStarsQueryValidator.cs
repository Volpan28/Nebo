using FluentValidation;

namespace AstroMonitor.Application.Features.Stars.Queries.GetVisible;

public class GetVisibleStarsQueryValidator : AbstractValidator<GetVisibleStarsQuery>
{
    public GetVisibleStarsQueryValidator()
    {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("Latitude must be between -90 and 90 degrees.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude must be between -180 and 180 degrees.");

        RuleFor(x => x.MinAltitude)
            .InclusiveBetween(-90, 90)
            .When(x => x.MinAltitude.HasValue)
            .WithMessage("MinAltitude must be between -90 and 90 degrees.");
    }
}