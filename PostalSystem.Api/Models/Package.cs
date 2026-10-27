namespace PostalSystem.Api.Models
{
    public class Package
    {
        public int PackageId { get; set; }
        public Guid TrackingGuid { get; set; } = Guid.NewGuid();
        public string DeliveryOTP { get; set; } = string.Empty;
        public int OriginWarehouseId { get; set; }
        public int CurrentWarehouseId { get; set; }
        public int FinalDestinationWarehouseId { get; set; }
        public string DestinationAddress { get; set; } = string.Empty;
        public int? CurrentPostmanId { get; set; }
        public bool IsFlaggedAsFragile { get; set; } = false;
        public string Status { get; set; } = "Registered";
        public bool IsCanceled { get; set; } = false;
        
        public Warehouse? OriginWarehouse { get; set; }
        public Warehouse? CurrentWarehouse { get; set; }
        public Warehouse? FinalDestinationWarehouse { get; set; }
        public Postman? CurrentPostman { get; set; }
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}