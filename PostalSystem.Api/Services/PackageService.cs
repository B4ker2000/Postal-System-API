using Azure;
using Microsoft.EntityFrameworkCore;
using PostalSystem.Api.Data;
using PostalSystem.Api.Dtos;
using PostalSystem.Api.Models;

namespace PostalSystem.Api.Services
{
    public class PackageService : IPackageService
    {
        private readonly PostalSystemDbContext _context;
        private readonly ILogger<PackageService> _logger;

        public PackageService(PostalSystemDbContext context, ILogger<PackageService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult> RegisterPackageAsync(int postmanId, RegisterPackageDto dto)
        {
            // 1. Validate the postman and get their home warehouse
            var postman = await _context.Postmen
                .FirstOrDefaultAsync(p => p.PostmanId == postmanId && p.IsActive);

            if (postman == null)
                return new ServiceResult(false, "Active postman not found.");

            // 2. Create the Package entity
            // Also generate a random tracking GUID and a 6-digit OTP code
            var package = new Package
            {
                TrackingGuid = Guid.NewGuid(), //gen in db
                DeliveryOTP = new Random().Next(100000, 999999).ToString(),
                FinalDestinationCityId = dto.FinalDestinationCityId,
                DestinationAddress = dto.DestinationAddress,
                IsFlaggedAsFragile = dto.IsFlaggedAsFragile,
                CalculatedStatus = PackageStatus.Registered,
                IsCanceled = false
            };

            _context.Packages.Add(package);

            // 3. Create the "Registered" Event
            var registeredEventType = await _context.EventTypes
                .FirstOrDefaultAsync(et => et.Code == EventTypeCode.Registered);

            if (registeredEventType == null)
                return new ServiceResult(false, "Event type 'PR1' not found in database.");

            var registrationEvent = new Event
            {
                PackageId = package.TrackingGuid,
                EventType = registeredEventType,
                PostmanId = postmanId,
                WarehouseId = postman.WarehouseId, // This marks the Origin!
                Timestamp = DateTime.UtcNow,
                Details = await GenerateEventDetailsAsync(EventTypeCode.Registered, postmanId, postman.WarehouseId)
            };

            _context.Events.Add(registrationEvent);

            // 4. Save everything in one transaction
            await _context.SaveChangesAsync();

            return new ServiceResult(true, "Package registered successfully!",
                   new RegisterPackageResponse(package.TrackingGuid, package.DeliveryOTP));
        }

        public async Task<ServiceResult> PickupPackageAsync(PickupRequest request)
        {
            var package = await _context.Packages.FindAsync(request.TrackingGuid);
            var postman = await _context.Postmen.FindAsync(request.PostmanId);

            if (package == null)
                return new ServiceResult(false, "Package not found.");
            if (postman == null || !postman.IsActive)
                return new ServiceResult(false, "Active postman not found.");

            var lastEvent = await _context.Events
                .Where(e => e.PackageId == package.TrackingGuid)
                .OrderByDescending(e => e.Timestamp)
                .FirstOrDefaultAsync();

            // Verify the package is at the postman's assigned warehouse
            if (lastEvent == null || lastEvent?.WarehouseId != postman.WarehouseId)
                return new ServiceResult(false, "Package is not at the postman's assigned warehouse.");

            var pickupEventType = await _context.EventTypes.FirstAsync(et => et.Code == EventTypeCode.PickedUp);

            var pickupEvent = new Event
            {
                PackageId = package.TrackingGuid,
                EventTypeId = pickupEventType.EventTypeId,
                PostmanId = postman.PostmanId,
                WarehouseId = postman.WarehouseId,
                Timestamp = DateTime.UtcNow,
                Details = await GenerateEventDetailsAsync(EventTypeCode.PickedUp, postman.PostmanId, postman.WarehouseId)
            };

            package.CalculatedStatus = PackageStatus.InTransit;
            _context.Events.Add(pickupEvent);
            await _context.SaveChangesAsync();
            return new ServiceResult(true, "Package picked up successfully!");
        }

        public async Task<ServiceResult> TransferPackageAsync(TransferRequest request)
        {
            var package = await _context.Packages.FindAsync(request.TrackingGuid);
            if (package == null || package.CalculatedStatus != PackageStatus.InTransit)
                return new ServiceResult(false, "Package status doesn't allow transfer to a new warehouse.");

            var warehouse = await _context.Warehouses.FindAsync(request.WarehouseId);
            if (warehouse == null)
                return new ServiceResult(false, "Warehouse not found.");

            var warehouseEntryType = await _context.EventTypes.FirstAsync(et => et.Code == EventTypeCode.InWarehouse);

            // last event should be `InTransit`

            var transferEvent = new Event
            {
                PackageId = package.TrackingGuid,
                EventTypeId = warehouseEntryType.EventTypeId,
                PostmanId = request.PostmanId,
                WarehouseId = warehouse.WarehouseId,
                Timestamp = DateTime.UtcNow,
                Details = await GenerateEventDetailsAsync(EventTypeCode.InWarehouse, request.PostmanId, warehouse.WarehouseId)
            };

            package.CalculatedStatus = PackageStatus.InWarehouse;
            _context.Events.Add(transferEvent);
            await _context.SaveChangesAsync();
            return new ServiceResult(true, "Package transfered successfully.");
        }

        public async Task<ServiceResult> ConfirmDeliveryAsync(ConfirmDeliveryRequest request)
        {
            var package = await _context.Packages.FindAsync(request.TrackingGuid);
            var postman = await _context.Postmen.FindAsync(request.PostmanId);

            if (package == null)
                return new ServiceResult(false, "Package not found.");
            if (postman == null || !postman.IsActive)
                return new ServiceResult(false, "Postman not found.");

            // Verify the secret OTP
            if (package.DeliveryOTP != request.Otp)
                return new ServiceResult(false, "Entered OTP code is wrong.");

            var lastEvent = await _context.Events
                .Where(e => e.PackageId == package.TrackingGuid)
                .OrderByDescending(e => e.Timestamp)
                .FirstOrDefaultAsync();

            // destination city id check
            // confirm delivery check for others

            // Verify the last event was a pickup (PP2)
            var pickupEventType = await _context.EventTypes
                .FirstOrDefaultAsync(et => et.Code == EventTypeCode.PickedUp);
            if (lastEvent == null || lastEvent.EventTypeId != pickupEventType?.EventTypeId)
                return new ServiceResult(false, "Package isn't picked up yet!");

            var deliveryEventType = await _context.EventTypes.FirstAsync(et => et.Code == EventTypeCode.Delivered);

            var deliveryEvent = new Event
            {
                PackageId = package.TrackingGuid,
                EventTypeId = deliveryEventType.EventTypeId,
                PostmanId = postman.PostmanId,
                WarehouseId = lastEvent.WarehouseId,
                Timestamp = DateTime.UtcNow,
                Details = await GenerateEventDetailsAsync(EventTypeCode.Delivered, postman.PostmanId, lastEvent.WarehouseId)
            };

            package.CalculatedStatus = PackageStatus.Delivered;
            _context.Events.Add(deliveryEvent);
            await _context.SaveChangesAsync();
            return new ServiceResult(true, "Package successfully delivered.");
        }

        public async Task<ServiceResult> CancelPackageAsync(CancelPackageRequest request)
        {
            var package = await _context.Packages.FindAsync(request.TrackingGuid);
            var postman = await _context.Postmen.FindAsync(request.PostmanId);

            if (package == null)
                return new ServiceResult(false, "Package not found.");
            if (postman == null || !postman.IsActive)
                return new ServiceResult(false, "Active postman not found.");
            if (package.IsCanceled)
                return new ServiceResult(false, "Package is already canceled.");
            
            // 1. Find the last event and include the EventType to check the Code
            var lastEvent = await _context.Events
                // .Include(e => e.EventType) // "PR1" code check!
                .Where(e => e.PackageId == package.TrackingGuid)
                .OrderByDescending(e => e.Timestamp)
                .Select(x => new
                {
                    EventTypeCode = x.EventType.Code, // SENIOR WRITTEN CODE!
                    x.WarehouseId,
                })
                .FirstOrDefaultAsync();

            // 2. Only cancel if the last event was "PR1" (Registered)
            if (lastEvent == null || lastEvent.EventTypeCode != EventTypeCode.Registered) 
                return new ServiceResult(false, "Packages can only be canceled if they have not yet left the registration warehouse.");

            // 3. Verify the postman belongs to that registration warehouse
            if (lastEvent.WarehouseId != postman.WarehouseId)
                return new ServiceResult(false, "Packages can only be canceled by postmen assigned to the registration warehouse.");

            // 4. Mark as canceled and log the "PC2" event
            package.IsCanceled = true;
            var cancelEventType = await _context.EventTypes.FirstAsync(et => et.Code == EventTypeCode.Canceled);

            var cancelEvent = new Event
            {
                PackageId = package.TrackingGuid,
                EventTypeId = cancelEventType.EventTypeId,
                PostmanId = postman.PostmanId,
                WarehouseId = lastEvent.WarehouseId,
                Timestamp = DateTime.UtcNow,
                Details = await GenerateEventDetailsAsync(EventTypeCode.Canceled, postman.PostmanId, lastEvent.WarehouseId)
            };

            _context.Events.Add(cancelEvent);
            await _context.SaveChangesAsync();

            return new ServiceResult(true, "Package successfully canceled.");
        }

        private async Task<string> GenerateEventDetailsAsync(string eventCode, int? postmanId, int? warehouseId)
        {
            var postman = postmanId.HasValue
                ? await _context.Postmen.FindAsync(postmanId.Value)
                : null;

            var warehouse = warehouseId.HasValue
                ? await _context.Warehouses.FindAsync(warehouseId.Value)
                : null;

            string pName = postman?.Name ?? "Unknown Postman";
            string wName = warehouse?.WarehouseName ?? "Unknown Warehouse";

            return eventCode switch
            {
                EventTypeCode.Registered  => $"Package registered by {pName} at {wName}.",
                EventTypeCode.PickedUp    => $"Picked up by postman {pName} from {wName}.",
                EventTypeCode.InWarehouse => $"Stored at {wName} by postman {pName}.",
                EventTypeCode.Delivered   => $"Delivered by postman {pName} (Last stop: {wName}).",
                EventTypeCode.Canceled    => $"Package canceled by {pName}.",
                _                         => "Event logged in system."
            };
        }
    }
}