using FluentValidation;
using PostalSystem.Api.Dtos;

public class PickupRequestValidator : AbstractValidator<PickupRequest>
{
    public PickupRequestValidator()
    {
        RuleFor(x => x.TrackingGuid).NotEmpty();
        RuleFor(x => x.PostmanId).GreaterThan(0);
    }
}