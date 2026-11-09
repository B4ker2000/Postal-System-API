using PostalSystem.Api.Dtos;

namespace PostalSystem.Api.Services
{
    public interface IPostmanService
    {
        Task<ServiceResult> LoginAsync(LoginRequest request);
    }
}