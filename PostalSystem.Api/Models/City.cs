namespace PostalSystem.Api.Models
{
    public class City
    {
        public int CityId { get; set; }
        public string CityName { get; set; } = string.Empty;
        
        //public ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>(); // many to many relation
        public Warehouse? Warehouse { get; set; }
    }
}