namespace PostalSystem.Api.Models
{
    public class Postman
    {
        public int PostmanId { get; set; }
        public int WarehouseId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; } = true;
        
        public Warehouse? Warehouse { get; set; }
        public ICollection<Package> Packages { get; set; } = new List<Package>();
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}