namespace PostalSystem.Api.Dtos
{    
    // Helper DTO for the request
    public record PickupRequest(
        Guid TrackingGuid, 
        int PostmanId
    );
}