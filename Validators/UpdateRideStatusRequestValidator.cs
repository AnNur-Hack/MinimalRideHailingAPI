using FluentValidation;
using MinimalRideHailingAPI.DTOs.Requests;

namespace MinimalRideHailingAPI.Validators;

public class UpdateRideStatusRequestValidator : AbstractValidator<UpdateRideStatusRequest>
{
    public UpdateRideStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid ride status.");
    }
}