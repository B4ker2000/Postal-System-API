namespace PostalSystem.Api.Dtos
{
    public class RegisterPackageRequest
    {
        public int FinalDestinationCityId { get; set; }
        public string DestinationAddress { get; set; } = string.Empty;
        public bool IsFlaggedAsFragile { get; set; }
    }
}