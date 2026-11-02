//using System.Diagnostics;

namespace PostalSystem.Api.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public Guid PackageId { get; set; }
        public int EventTypeId { get; set; }
        public int PostmanId { get; set; }
        public int WarehouseId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Details { get; set; }

        public Package Package { get; set; } = null!;
        public EventType EventType { get; set; } = null!;
        public Postman Postman { get; set; } = null!;
        public Warehouse Warehouse { get; set; } = null!;
    }
}