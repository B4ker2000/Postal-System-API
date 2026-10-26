namespace PostalSystem.Api.Models
{
    public class Warehouse
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int CityId { get; set; }

        public City? City { get; set; }
        public ICollection<Postman> Postmen { get; set; } = new List<Postman>();
        public ICollection<Package> PackagesOrigin { get; set; } = new List<Package>();
        public ICollection<Package> PackagesCurrent { get; set; } = new List<Package>();
        public ICollection<Package> PackagesDestination { get; set; } = new List<Package>();
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}