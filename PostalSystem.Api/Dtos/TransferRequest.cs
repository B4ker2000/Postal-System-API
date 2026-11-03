namespace PostalSystem.Api.Dtos
{    
    public record TransferRequest(
        Guid TrackingGuid,
        int PostmanId, 
        int WarehouseId // SENIOR SUGGESTION: Remove warehouse here. Ask for more directions! 
    ); 
}