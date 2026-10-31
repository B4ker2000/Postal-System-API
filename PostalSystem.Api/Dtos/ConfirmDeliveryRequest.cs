namespace PostalSystem.Api.Dtos;

public record ConfirmDeliveryRequest(
    Guid TrackingGuid, 
    string Otp
);