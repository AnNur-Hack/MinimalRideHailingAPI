using FluentValidation;
using MinimalRideHailingAPI.DTOs.Requests;

namespace MinimalRideHailingAPI.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required");
        
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required");
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email address");
        
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MaximumLength(11)
            .WithMessage("Phone number is required");
        
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters.")
            .Matches(@"[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]")
            .WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"\d")
            .WithMessage("Password must contain at least one number.")
            .Matches(@"[^a-zA-Z0-9]")
            .WithMessage("Password must contain at least one special character.");
        
        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Invalid user role.");
        
    }
}