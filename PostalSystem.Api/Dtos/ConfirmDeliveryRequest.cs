namespace PostalSystem.Api.Dtos
{
    public record ConfirmDeliveryRequest(
        Guid TrackingGuid, 
        int PostmanId,  // Temporary as we don't have a user login/registration system yet!
        string Otp
    );
}