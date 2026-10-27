using PostalSystem.Api.Dtos;
using PostalSystem.Api.Models;

namespace PostalSystem.Api.Services
{
    public interface IPackageService
    {
        Task<Package> RegisterPackageAsync(int postmanId, RegisterPackageDto dto);
    }
}