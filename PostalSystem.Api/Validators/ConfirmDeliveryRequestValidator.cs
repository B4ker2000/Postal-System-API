using FluentValidation;
using PostalSystem.Api.Dtos;

public class ConfirmDeliveryRequestValidator : AbstractValidator<ConfirmDeliveryRequest>
{
    public ConfirmDeliveryRequestValidator()
    {
        RuleFor(x => x.TrackingGuid).NotEmpty();
        RuleFor(x => x.Otp).NotEmpty().Length(6);
    }
}