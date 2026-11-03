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
            var postman = await _context.Postmen
                .FirstOrDefaultAsync(p => p.PostmanId == postmanId && p.IsActive);

            // Validate the postman and get their home warehouse
            if (postman == null || !postman.IsActive || postman.Warehouse == null)
            {
                _logger.LogWarning("Registration failed: Active postman {PostmanId} not found.", postmanId);
                return new ServiceResult(false, "Active postman not found.");
            }

            // 2. Create the Package entity
            // Also generate a random a 6-digit OTP code
            // TrackingGuid is now handled by SQL Server via newid()
            var package = new Package
            {
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
            {
                _logger.LogError("Database Configuration Error: Event type {Code} not found.", EventTypeCode.Registered);
                return new ServiceResult(false, $"Event type '{EventTypeCode.Registered}' not found in database.");
            }

            var registrationEvent = new Event
            {
                EventType = registeredEventType,
                PostmanId = postmanId,
                WarehouseId = postman.WarehouseId, // This marks the Origin!
                Timestamp = DateTime.UtcNow,
                Details = GenerateEventDetailsAsync(EventTypeCode.Registered, postman.Name, postman.Warehouse.WarehouseName)
            };

            _context.Events.Add(registrationEvent);

            // 4. Save everything in one transaction
            await _context.SaveChangesAsync();

            _logger.LogInformation("Package {Guid} successfully registered by postman {PostmanId}.", package.TrackingGuid, postmanId);
            return new ServiceResult(true, "Package registered successfully!",
                   new RegisterPackageResponse(package.TrackingGuid, package.DeliveryOTP));
        }

        public async Task<ServiceResult> PickupPackageAsync(PickupRequest request)
        {
            var postman = await _context.Postmen.FindAsync(request.PostmanId);
            var package = await _context.Packages.FindAsync(request.TrackingGuid);

            if (postman == null || !postman.IsActive || postman.Warehouse == null)
            {
                _logger.LogWarning("Pickup failed: Active postman {PostmanId} not found.", request.PostmanId);
                return new ServiceResult(false, "Active postman not found.");
            }

            if (package == null)
            {
                _logger.LogWarning("Pickup failed: Package {Guid} not found.", request.TrackingGuid);
                return new ServiceResult(false, "Package not found.");
            }

            var lastEvent = await _context.Events
                .Include(e => e.EventType)
                .Where(e => e.PackageId == package.TrackingGuid)
                .OrderByDescending(e => e.Timestamp)
                .FirstOrDefaultAsync();

            // Verify the package is at the postman's assigned warehouse
            if (lastEvent == null || lastEvent.WarehouseId != postman.WarehouseId)
            {
                _logger.LogWarning("Pickup failed: Package {Guid} is not at postman {PostmanId}'s assigned warehouse.", request.TrackingGuid, request.PostmanId);
                return new ServiceResult(false, "Package is not at the postman's assigned warehouse.");
            }

            if (lastEvent.EventType.Code != EventTypeCode.Registered && lastEvent.EventType.Code != EventTypeCode.InWarehouse)
            {
                _logger.LogWarning("Pickup failed: Package {Guid} is in an invalid state for pickup (Last Event: {Code}).", request.TrackingGuid, lastEvent.EventType.Code);
                return new ServiceResult(false, "Package is not in a state that allows for pickup.");
            }

            if (lastEvent.EventType.Code == EventTypeCode.Delivered)
            {
                _logger.LogWarning("Pickup failed: Package {Guid} was already delivered.", request.TrackingGuid);
                return new ServiceResult(false, "Package is already delivered.");
            }

            var pickupEventType = await _context.EventTypes.FirstAsync(et => et.Code == EventTypeCode.PickedUp);

            var pickupEvent = new Event
            {
                PackageId = package.TrackingGuid,
                EventTypeId = pickupEventType.EventTypeId,
                PostmanId = postman.PostmanId,
                WarehouseId = postman.WarehouseId,
                Timestamp = DateTime.UtcNow,
                Details = GenerateEventDetailsAsync(EventTypeCode.PickedUp, postman.Name, postman.Warehouse.WarehouseName)
            };

            package.CalculatedStatus = PackageStatus.InTransit;
            _context.Events.Add(pickupEvent);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Package {Guid} successfully picked up by postman {PostmanId}.", package.TrackingGuid, request.PostmanId);
            return new ServiceResult(true, "Package picked up successfully!");
        }

        public async Task<ServiceResult> TransferPackageAsync(TransferRequest request)
        {
            var postman = await _context.Postmen.FindAsync(request.PostmanId);
            var package = await _context.Packages.FindAsync(request.TrackingGuid);
            
            if (postman == null || !postman.IsActive || postman.Warehouse == null)
            {
                _logger.LogWarning("Transfer failed: Active postman {PostmanId} not found.", request.PostmanId);
                return new ServiceResult(false, "Active postman not found.");
            }
            
            if (package == null)
            {
                _logger.LogWarning("Transfer failed: Package {Guid} not found.", request.TrackingGuid);
                return new ServiceResult(false, "Package not found.");
            }

            if (package.CalculatedStatus != PackageStatus.InTransit)
            {
                _logger.LogWarning("Transfer failed: Package {Guid} is not currently In Transit.", request.TrackingGuid);
                return new ServiceResult(false, "Package status doesn't allow transfer to a new warehouse.");
            }

            var warehouse = await _context.Warehouses.FindAsync(request.WarehouseId);
            if (warehouse == null)
            {
                _logger.LogWarning("Transfer failed: Destination warehouse {WarehouseId} not found.", request.WarehouseId);
                return new ServiceResult(false, "Warehouse not found.");
            }

            var lastEvent = await _context.Events
                .Include(e => e.EventType)
                .Where(e => e.PackageId == package.TrackingGuid)
                .OrderByDescending(e => e.Timestamp)
                .FirstOrDefaultAsync();
            
            if (lastEvent == null || lastEvent.EventType.Code != EventTypeCode.PickedUp)
            {
                _logger.LogWarning("Transfer failed: Package {Guid} last event was not 'Picked Up'.", request.TrackingGuid);
                return new ServiceResult(false, $"Last package status is not '{EventTypeCode.PickedUp}'.");
            }

            if (lastEvent.EventType.Code == EventTypeCode.Delivered)
            {
                _logger.LogWarning("Package Transfer failed: Package {Guid} was already delivered.", request.TrackingGuid);
                return new ServiceResult(false, "Package is already delivered.");
            }

            var warehouseEntryType = await _context.EventTypes.FirstAsync(et => et.Code == EventTypeCode.InWarehouse);

            var transferEvent = new Event
            {
                PackageId = package.TrackingGuid, // this stays right?
                EventTypeId = warehouseEntryType.EventTypeId,
                PostmanId = request.PostmanId,
                WarehouseId = warehouse.WarehouseId,
                Timestamp = DateTime.UtcNow,
                Details = GenerateEventDetailsAsync(EventTypeCode.InWarehouse, postman.Name, postman.Warehouse.WarehouseName)
            };

            package.CalculatedStatus = PackageStatus.InWarehouse;
            _context.Events.Add(transferEvent);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Package {Guid} successfully transferred by postman {PostmanId}.", package.TrackingGuid, request.PostmanId);
            return new ServiceResult(true, "Package transferred successfully.");
        }

        public async Task<ServiceResult> ConfirmDeliveryAsync(ConfirmDeliveryRequest request)
        {
            var postman = await _context.Postmen.FindAsync(request.PostmanId);
            var package = await _context.Packages.FindAsync(request.TrackingGuid);
            
            if (postman == null || !postman.IsActive || postman.Warehouse == null)
            {
                _logger.LogWarning("Package Delivery failed: Active postman {PostmanId} not found.", request.PostmanId);
                return new ServiceResult(false, "Postman not found.");
            }

            if (package == null)
            {
                _logger.LogWarning("Package Delivery failed: Package {Guid} not found.", request.TrackingGuid);
                return new ServiceResult(false, "Package not found.");
            }

            // Verify the secret OTP
            if (package.DeliveryOTP != request.Otp)
            {
                _logger.LogError("Package Delivery failed: OTP code does not match for package {Guid}.", request.TrackingGuid);
                return new ServiceResult(false, "Entered OTP code is wrong.");
            }

            var lastEvent = await _context.Events
                .Where(e => e.PackageId == package.TrackingGuid)
                .OrderByDescending(e => e.Timestamp)
                .Select(e => new
                {
                    EventTypeCode = e.EventType.Code,
                    e.Warehouse
                })
                .FirstOrDefaultAsync();

            if (lastEvent == null)
            {
                _logger.LogWarning("Package Delivery failed: No event history for {Guid}.", request.TrackingGuid);
                return new ServiceResult(false, "Package history not found.");
            }

            if (lastEvent.EventTypeCode == EventTypeCode.Delivered)
            {
                _logger.LogWarning("Package Delivery failed: Package {Guid} was already delivered.", request.TrackingGuid);
                return new ServiceResult(false, "Package is already delivered.");
            }

            if (lastEvent.Warehouse == null || package.FinalDestinationCityId != lastEvent.Warehouse.CityId)
            {
                _logger.LogWarning("Package Delivery failed: Postman is in the wrong city for package {Guid}.", request.TrackingGuid);
                return new ServiceResult(false, "Postman does not have the privileges to deliver packages in a different city.");
            }

            // Verify the last event was a pickup (PP2)
            if (lastEvent.EventTypeCode != EventTypeCode.PickedUp)
            {
                _logger.LogWarning("Package Delivery failed: Package {Guid} was not picked up.", request.TrackingGuid);
                return new ServiceResult(false, "Package isn't picked up yet!");
            }

            var deliveryEventType = await _context.EventTypes.FirstAsync(et => et.Code == EventTypeCode.Delivered);

            var deliveryEvent = new Event
            {
                PackageId = package.TrackingGuid,
                EventTypeId = deliveryEventType.EventTypeId,
                PostmanId = postman.PostmanId,
                WarehouseId = lastEvent.Warehouse.WarehouseId,
                Timestamp = DateTime.UtcNow,
                Details = GenerateEventDetailsAsync(EventTypeCode.Delivered, postman.Name, postman.Warehouse.WarehouseName)
            };

            package.CalculatedStatus = PackageStatus.Delivered;
            _context.Events.Add(deliveryEvent);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Package {Guid} successfully delivered by postman {PostmanId}.", package.TrackingGuid, postman.PostmanId);
            return new ServiceResult(true, "Package successfully delivered.");
        }

        public async Task<ServiceResult> CancelPackageAsync(CancelPackageRequest request)
        {
            var postman = await _context.Postmen.FindAsync(request.PostmanId);
            var package = await _context.Packages.FindAsync(request.TrackingGuid);

            if (postman == null || !postman.IsActive || postman.Warehouse == null)
            {
                _logger.LogWarning("Package Cancellation failed: Active postman {PostmanId} not found.", request.PostmanId);
                return new ServiceResult(false, "Active postman not found.");
            }

            if (package == null)
            {
                _logger.LogWarning("Package Cancellation failed: Package {Guid} not found.", request.TrackingGuid);
                return new ServiceResult(false, "Package not found.");
            }

            if (package.IsCanceled)
            {
                _logger.LogWarning("Package Cancellation failed: Package {Guid} is already canceled.", request.TrackingGuid);
                return new ServiceResult(false, "Package is already canceled.");
            }
            
            // Find the last event and include the EventType to check the Code
            var lastEvent = await _context.Events
                .Where(e => e.PackageId == package.TrackingGuid)
                .OrderByDescending(e => e.Timestamp)
                .Select(e => new
                {
                    EventTypeCode = e.EventType.Code, 
                    e.WarehouseId,
                    e.Warehouse.WarehouseName
                })
                .FirstOrDefaultAsync();

            if (lastEvent == null)
            {
                _logger.LogWarning("Package Cancellation failed: No event history for {Guid}.", request.TrackingGuid);
                return new ServiceResult(false, "Package history not found.");
            }

            // Only cancel if the last event was "PR1" (Registered)
            if (lastEvent.EventTypeCode != EventTypeCode.Registered) 
            {
                _logger.LogError("Package Cancellation failed: Package {Guid} has already left the registration warehouse at {WarehouseId}.", request.TrackingGuid, lastEvent.WarehouseName);
                return new ServiceResult(false, "Packages can only be canceled if they have not yet left the registration warehouse.");
            }

            // Verify the postman belongs to that registration warehouse
            if (lastEvent.WarehouseId != postman.WarehouseId)
            {
                _logger.LogError("Package Cancellation failed: Packages can only be Canceled by postmen assigned to the registration warehouse at {WarehouseId}.", lastEvent.WarehouseName);
                return new ServiceResult(false, "Packages can only be canceled by postmen assigned to the registration warehouse.");
            }

            // Mark as canceled and log the "PC2" event
            package.IsCanceled = true;
            var cancelEventType = await _context.EventTypes.FirstAsync(et => et.Code == EventTypeCode.Canceled);

            var cancelEvent = new Event
            {
                PackageId = package.TrackingGuid,
                EventTypeId = cancelEventType.EventTypeId,
                PostmanId = postman.PostmanId,
                WarehouseId = lastEvent.WarehouseId,
                Timestamp = DateTime.UtcNow,
                Details = GenerateEventDetailsAsync(EventTypeCode.Canceled, postman.Name, postman.Warehouse.WarehouseName)
            };

            _context.Events.Add(cancelEvent);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Package {Guid} was successfully canceled.", request.TrackingGuid);
            return new ServiceResult(true, "Package successfully canceled.");
        }

        private string GenerateEventDetailsAsync(string eventCode, string pName, string wName)
        {
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