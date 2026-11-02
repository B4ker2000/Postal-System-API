using FluentValidation;
using PostalSystem.Api.Dtos;

public class CancelPackageRequestValidator : AbstractValidator<CancelPackageRequest>
{
    public CancelPackageRequestValidator()
    {
        RuleFor(x => x.TrackingGuid).NotEmpty();
        RuleFor(x => x.PostmanId).GreaterThan(0);
    }
}