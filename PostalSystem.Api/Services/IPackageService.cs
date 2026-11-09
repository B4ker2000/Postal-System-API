using PostalSystem.Api.Dtos;

namespace PostalSystem.Api.Services
{
    public interface IPackageService
    {
        Task<ServiceResult> RegisterPackageAsync(RegisterPackageRequest dto, int postmanId);
        Task<ServiceResult> PickupPackageAsync(Guid trackingGuid, int postmanId);
        Task<ServiceResult> TransferPackageAsync(Guid trackingGuid, int postmanId);
        Task<ServiceResult> ConfirmDeliveryAsync(ConfirmDeliveryRequest request, int postmanId);
        Task<ServiceResult> CancelPackageAsync(Guid trackingGuid, int postmanId);
        Task<ServiceResult<List<EventTrackingDto>>> GetPackageHistoryAsync(Guid trackingGuid);
    }
}