using FluentValidation;

namespace AstroMonitor.Application.Features.Asteroids.Commands.CreateAsteroids;

public class CreateAsteroidCommandValidator : AbstractValidator<CreateAsteroidCommand>
{
    public  CreateAsteroidCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(40).WithMessage("Name must not exceed 40 characters.");

        RuleFor(x => x.MinDiameterMeters)
            .GreaterThan(0).WithMessage("Min diameter meters must be greater than zero.");
        
        RuleFor(y => y.MaxDiameterMeters)
            .GreaterThan(0).WithMessage("Max diameter meters must be greater than zero.")
            .GreaterThanOrEqualTo(x => x.MinDiameterMeters)
            .WithMessage("Max diameter meters must be greater than minimum diameter meters.");
        
        RuleFor(y => y.ClosestApproachDate)
            .NotEmpty().WithMessage("ClosestApproachDate is required.")
            .NotEqual(default(DateTimeOffset)).WithMessage("ClosestApproachDate invalid.");
        
        RuleFor(y => y.RelativeVelocityKmPerSec)
            .GreaterThan(0).WithMessage("RelativeVelocityKmPerSec must be greater than zero.");
    }
}