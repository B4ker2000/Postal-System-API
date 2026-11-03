namespace PostalSystem.Api.Dtos
{
    public record RegisterPackageResponse(
        Guid TrackingGuid, 
        string DeliveryOTP
    );
}