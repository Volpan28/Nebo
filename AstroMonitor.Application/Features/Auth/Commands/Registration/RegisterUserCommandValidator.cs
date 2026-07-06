using FluentValidation;

namespace AstroMonitor.Application.Features.Auth.Commands.Registration;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MinimumLength(3).WithMessage("First name must be at least 3 characters long.");
        
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MinimumLength(3).WithMessage("Last name must be at least 3 characters long.");
        
        RuleFor(x=> x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password cannot be empty.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"[0-9]").WithMessage("Password must contain numbers.")
            .Matches(@"[a-zA-Z]").WithMessage("Password must contain letters.")
            .Matches(@"[!@#$%^&*()_+=\[\]{};':""\\|,.<>/?~-]")
            .WithMessage("Password must contain at least one special character.");
    }
}