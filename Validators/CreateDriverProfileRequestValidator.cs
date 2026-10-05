using FluentValidation;
using MinimalRideHailingAPI.DTOs.Requests;

namespace MinimalRideHailingAPI.Validators;

public class CreateDriverProfileRequestValidator : AbstractValidator<CreateDriverProfileRequest>
{
    public CreateDriverProfileRequestValidator()
    {
        RuleFor(x => x.LicenseNumber)
            .NotEmpty()
            .WithMessage("License number is required.");
    }
}