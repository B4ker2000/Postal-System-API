using PostalSystem.Api.Dtos;
using PostalSystem.Api.Models;

namespace PostalSystem.Api.Services
{
    public interface IPackageService
    {
        Task<ServiceResult> RegisterPackageAsync(RegisterPackageDto dto);
        Task<ServiceResult> PickupPackageAsync(PickupRequest request);
        Task<ServiceResult> TransferPackageAsync(TransferRequest request);
        Task<ServiceResult> ConfirmDeliveryAsync(ConfirmDeliveryRequest request);
        Task<ServiceResult> CancelPackageAsync(CancelPackageRequest request);
    }
}