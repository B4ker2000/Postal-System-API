namespace PostalSystem.Api.Dtos
{
    public record ConfirmDeliveryRequest(
        Guid TrackingGuid, 
        int PostmanId, 
        string Otp
    );
}