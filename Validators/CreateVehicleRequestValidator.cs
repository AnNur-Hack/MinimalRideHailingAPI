using FluentValidation;
using MinimalRideHailingAPI.DTOs.Requests;

namespace MinimalRideHailingAPI.Validators;

public class CreateVehicleRequestValidator : AbstractValidator<CreateVehicleRequest>
{
    public CreateVehicleRequestValidator()
    {
        RuleFor(x => x.Make)
            .NotEmpty()
            .WithMessage("Vehicle make is required.");

        RuleFor(x => x.Model)
            .NotEmpty()
            .WithMessage("Vehicle model is required.");

        RuleFor(x => x.Color)
            .NotEmpty()
            .WithMessage("Vehicle color is required.");

        RuleFor(x => x.Year)
            .NotEmpty()
            .WithMessage("Vehicle year is required.");

        RuleFor(x => x.PlateNumber)
            .NotEmpty()
            .WithMessage("Plate number is required.");

        RuleFor(x => x.VehicleType)
            .IsInEnum()
            .WithMessage("Invalid vehicle type.");
    }
}