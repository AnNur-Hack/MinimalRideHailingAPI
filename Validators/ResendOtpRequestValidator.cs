using FluentValidation;
using MinimalRideHailingAPI.DTOs.Requests;

namespace MinimalRideHailingAPI.Validators;

public class ResendOtpRequestValidator : AbstractValidator<ResendOtpRequest>
{
    public ResendOtpRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Enter a valid email address.");
    }
}