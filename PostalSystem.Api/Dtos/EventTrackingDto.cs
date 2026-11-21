namespace PostalSystem.Api.Dtos
{
    public record EventTrackingDto(
        int EventId,
        int EventTypeId,
        string EventTypeName,
        string PostmanName,
        string WarehouseName,
        string CityName,
        DateTime TimeStamp,
        string Details
    );
}