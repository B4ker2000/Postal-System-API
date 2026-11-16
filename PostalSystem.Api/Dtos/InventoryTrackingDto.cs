namespace PostalSystem.Api.Dtos
{
    public record InventoryTrackingDto(
        Guid TrackingGuid,
        int FinalDestinationCityId,
        string DestinationAddress,
        bool IsFlaggedAsFragile,
        DateTime TimeStamp
    );
}