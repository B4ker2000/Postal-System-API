using FluentValidation;
using PostalSystem.Api.Dtos;

public class TransferRequestValidator : AbstractValidator<TransferRequest>
{
    public TransferRequestValidator()
    {
        RuleFor(x => x.TrackingGuid).NotEmpty();
        RuleFor(x => x.PostmanId).GreaterThan(0);
        RuleFor(x => x.WarehouseId).GreaterThan(0);
    }
}