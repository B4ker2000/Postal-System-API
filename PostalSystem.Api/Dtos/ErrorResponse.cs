namespace PostalSystem.Api.Dtos
{
    public record ErrorResponse(
        int StatusCode, 
        string Message
    );
}