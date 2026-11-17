using PostalSystem.Api.Dtos;

namespace PostalSystem.Api.Services
{
    public interface IPackageService
    {
        Task<ServiceResult> RegisterPackageAsync(RegisterPackageRequest dto, int postmanId);
        Task<ServiceResult> PickupPackageAsync(GuidRequest Guid, int postmanId);
        Task<ServiceResult> TransferPackageAsync(GuidRequest Guid, int postmanId);
        Task<ServiceResult> ConfirmDeliveryAsync(ConfirmDeliveryRequest request, int postmanId);
        Task<ServiceResult> CancelPackageAsync(Guid trackingGuid, int postmanId);
        Task<ServiceResult<List<EventTrackingDto>>> GetPackageHistoryAsync(Guid trackingGuid);
        Task<ServiceResult<List<InventoryTrackingDto>>> GetWarehouseInventoryAsync(int postmanId);
        Task<ServiceResult<List<InventoryTrackingDto>>> GetInTransitPackagesAsync();
        Task<ServiceResult<List<InventoryTrackingDto>>> GetPickedUpPackagesAsync(int postmanId);
        Task<ServiceResult<List<InventoryTrackingDto>>> GetCancellablePackagesAsync(int postmanId);
    }
}