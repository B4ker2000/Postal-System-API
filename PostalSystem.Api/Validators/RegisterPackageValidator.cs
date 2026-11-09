using FluentValidation;
using PostalSystem.Api.Dtos;

namespace PostalSystem.Api.Validators;

public class RegisterPackageValidator : AbstractValidator<RegisterPackageRequest>
{
    public RegisterPackageValidator()
    {
        RuleFor(x => x.DestinationAddress).NotEmpty().MaximumLength(200);
        RuleFor(x => x.FinalDestinationCityId).GreaterThan(0);
    }
}