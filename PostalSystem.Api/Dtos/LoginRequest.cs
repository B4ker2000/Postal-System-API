namespace PostalSystem.Api.Dtos
{
    public record LoginRequest(
        string Name,
        string Phone
    );
}