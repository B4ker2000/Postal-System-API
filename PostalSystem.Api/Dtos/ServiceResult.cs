namespace PostalSystem.Api.Dtos
{
    public record ServiceResult(
        bool IsSuccess, 
        string Message, 
        object? Data = null
    );
}