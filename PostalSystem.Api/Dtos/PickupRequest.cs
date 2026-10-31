namespace PostalSystem.Api.Dtos
{    
    // Helper DTO for the request
    public class PickupRequest
    {
        public int PostmanId { get; set; }
        public Guid TrackingGuid { get; set; }
    }
}