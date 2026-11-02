namespace PostalSystem.Api.Dtos
{    
    public record TransferRequest(Guid TrackingGuid, int PostmanId, int WarehouseId); // --warehouse. Ask for more directions!
}