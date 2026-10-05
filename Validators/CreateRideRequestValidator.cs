using FluentValidation;
using MinimalRideHailingAPI.DTOs.Requests;

namespace MinimalRideHailingAPI.Validators;

public class CreateRideRequestValidator : AbstractValidator<CreateRideRequest>
{
    public CreateRideRequestValidator()
    {
        RuleFor(x => x.PickUpLocation)
            .NotEmpty()
            .WithMessage("Pick-up location is required.");

        RuleFor(x => x.Destination)
            .NotEmpty()
            .WithMessage("Destination is required.");
    }
}