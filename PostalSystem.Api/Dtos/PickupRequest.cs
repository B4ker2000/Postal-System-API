namespace PostalSystem.Api.Dtos
{    
    // Helper DTO for the request
    public record PickupRequest(
        Guid TrackingGuid, 
        int PostmanId // Temporary as we don't have a user login/registration system yet!
    );
}