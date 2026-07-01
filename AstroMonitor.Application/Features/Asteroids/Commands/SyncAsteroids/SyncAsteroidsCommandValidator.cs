using System.Globalization;
using FluentValidation;

namespace AstroMonitor.Application.Features.Asteroids.Commands.SyncAsteroids;

public class SyncAsteroidsCommandValidator : AbstractValidator<SyncAsteroidsCommand>
{
    public SyncAsteroidsCommandValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.")
            .Must(BeValiDate).WithMessage("Start date must be a valid date in format YYYY-MM-DD.");

        RuleFor(y => y.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .Must(BeValiDate).WithMessage("End date must be a valid date in format YYYY-MM-DD.");

        RuleFor(x => x)
            .Must(x => BeValiDateRange(x.StartDate, x.EndDate))
            .WithMessage("The end date must be after or equal to the start date, and the range cannot exceed 7 days.");
    }

    private bool BeValiDate(string date)
    {
        return DateTime.TryParseExact(date, "yyyy-mm-dd",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
    }

    private bool BeValiDateRange(string startStr, string endStr)
    {
        if (!DateTime.TryParse(startStr, out var start) || !DateTime.TryParse(endStr, out var end))
        {
            return false;
        }

        if (end < start)
        {
            return false;
        }
        
        return (end - start).TotalDays <= 7;
    }
}