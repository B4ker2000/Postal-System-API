namespace PostalSystem.Api.Models
{
    public enum PackageStatus
    {
        Registered = 1,
        InWarehouse = 2,
        InTransitToDestination = 3,
        Delivered = 4,
        Canceled = 5
    }
}