namespace PostalSystem.Api.Dtos
{    
    public record TransferRequest(
        Guid TrackingGuid,
        int PostmanId // Temporary as we don't have a user login/registration system yet!
    ); 
}