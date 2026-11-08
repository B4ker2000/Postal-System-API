namespace PostalSystem.Api.Dtos
{
    public record EventTrackingDto(
        int EventId,
        int EventTypeId,
        string EventTypeName,
        string PostmanName,
        string WarehouseName,
        DateTime TimeStamp,
        string Details
    );
}