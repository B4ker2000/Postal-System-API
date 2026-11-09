namespace PostalSystem.Api.Dtos
{
    public class RegisterPackageDto
    {
        public int FinalDestinationCityId { get; set; }
        public string DestinationAddress { get; set; } = string.Empty;
        public bool IsFlaggedAsFragile { get; set; }
        public int PostmanId { get; set; }  // Temporary as we don't have a user login/registration system yet!
    }
}