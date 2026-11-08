using FluentValidation;
using PostalSystem.Api.Dtos;

namespace PostalSystem.Api.Validators;

public class RegisterPackageValidator : AbstractValidator<RegisterPackageDto>
{
    public RegisterPackageValidator()
    {
        RuleFor(x => x.DestinationAddress).NotEmpty().MaximumLength(200);
        RuleFor(x => x.FinalDestinationCityId).GreaterThan(0);
        RuleFor(x => x.PostmanId).GreaterThan(0);
    }
}