namespace PostalSystem.Api.Models
{
    public class Warehouse
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int CityId { get; set; }
        public bool IsActive { get; set; } = true;

        public City City { get; set; } = null!;
        public ICollection<Postman> Postmen { get; set; } = new List<Postman>();
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}