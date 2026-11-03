namespace PostalSystem.Api.Dtos
{
    public record CancelPackageRequest(
        Guid TrackingGuid, 
        int PostmanId
    );
}