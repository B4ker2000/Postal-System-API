using FluentValidation;
using PostalSystem.Api.Dtos;

namespace PostalSystem.Api.Validators;

public class RegisterPackageValidator : AbstractValidator<RegisterPackageRequest> // was `RegisterPackageDto`
{
    public RegisterPackageValidator()
    {
        RuleFor(x => x.PackageDto.DestinationAddress).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PackageDto.FinalDestinationCityId).GreaterThan(0);
        RuleFor(x => x.PostmanId).GreaterThan(0);
    }
}