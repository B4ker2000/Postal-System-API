namespace PostalSystem.Api.Dtos
{
    // A small wrapper DTO to include the PostmanId
    public class RegisterPackageRequest
    {
        public int PostmanId { get; set; }
        public RegisterPackageDto PackageDto { get; set; } = new();
    }
}