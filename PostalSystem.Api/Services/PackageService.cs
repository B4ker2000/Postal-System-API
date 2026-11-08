using Azure.Core;
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

        public async Task<ServiceResult> RegisterPackageAsync(RegisterPackageDto dto)
        {
            var postman = await _context.Postmen
                .Include(p => p.Warehouse)
                .FirstOrDefaultAsync(p => p.PostmanId == dto.PostmanId && p.IsActive);
            
            var destinationCity = await _context.Cities
                .Include(c => c.Warehouse)
                .FirstOrDefaultAsync(c => c.CityId == dto.FinalDestinationCityId);

            // Validate the postman and check if their home warehouse exists or not
            if (postman == null || !postman.IsActive || postman.Warehouse == null)
            {
                _logger.LogWarning("Registration failed: Active postman {PostmanId} not found or is not active.", dto.PostmanId);
                return new ServiceResult(false, "Active postman not found or is not active.");
            }

            // Validate if the postman's warehouse is active or not
            if (!postman.Warehouse.IsActive)
            {
                _logger.LogWarning("Registration failed: Warehouse {WarehouseName} is currently under maintanance or is decommissioned.", postman.Warehouse.WarehouseName);
                return new ServiceResult(false, "The assigned warehouse is currently inactive.");
            }

            // Check if the destination city was set or not
            if (destinationCity == null)
            {
                _logger.LogWarning("Registration failed: The selected city is currently not yet supported.");
                return new ServiceResult(false, "The destination city is currently not yet supported.");
            }

            // Check if the destination city either has a warehouse or not or if it's active or not
            if (destinationCity.Warehouse == null || !destinationCity.Warehouse.IsActive)
            {
                _logger.LogWarning("Registration failed: The destination city does not have a warehouse yet, under maintanance, or is decommissioned");
                return new ServiceResult(false, "The destination city does not have a warehouse yet or is currently inactive.");
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
                Package = package,
                EventType = registeredEventType,
                PostmanId = dto.PostmanId,
                WarehouseId = postman.WarehouseId, // This marks the Origin!
                Timestamp = DateTime.UtcNow,
                Details = GenerateEventDetailsAsync(EventTypeCode.Registered, postman.Name, postman.Warehouse.WarehouseName)
            };

            _context.Events.Add(registrationEvent);

            // 4. Save everything in one transaction
            await _context.SaveChangesAsync();

            _logger.LogInformation("Package {Guid} successfully registered by postman {PostmanId}.", package.TrackingGuid, dto.PostmanId);
            return new ServiceResult(true, "Package registered successfully!",
                   new RegisterPackageResponse(package.TrackingGuid, package.DeliveryOTP));
        }

        public async Task<ServiceResult> PickupPackageAsync(PickupRequest request)
        {
            var postman = await _context.Postmen
                .Include(p => p.Warehouse)
                .FirstOrDefaultAsync(p => p.PostmanId == request.PostmanId && p.IsActive);
                
            var package = await _context.Packages.FindAsync(request.TrackingGuid);

            // Validate the postman and check if their home warehouse exists or not
            if (postman == null || !postman.IsActive || postman.Warehouse == null)
            {
                _logger.LogWarning("Pickup failed: Active postman {PostmanId} not found or is not active.", request.PostmanId);
                return new ServiceResult(false, "Active postman not found or is not active.");
            }

            // Validate if the postman's warehouse is active or not
            if (!postman.Warehouse.IsActive)
            {
                _logger.LogWarning("Pickup failed: Warehouse {WarehouseName} is currently under maintanance or is decommissioned.", postman.Warehouse.WarehouseName);
                return new ServiceResult(false, "The assigned warehouse is currently inactive.");
            }

            // Validate the package
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

            // Make sure the package was previously registered and it is inside a warehouse (not picked up yet)
            if (lastEvent.EventType.Code != EventTypeCode.Registered && lastEvent.EventType.Code != EventTypeCode.InWarehouse)
            {
                _logger.LogWarning("Pickup failed: Package {Guid} is in an invalid state for pickup (Last Event: {Code}).", request.TrackingGuid, lastEvent.EventType.Code);
                return new ServiceResult(false, "Package is not in a state that allows for pickup.");
            }

            // Make sure the package wasn't delivered previously
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
            var postman = await _context.Postmen
                .Include(p => p.Warehouse)
                .FirstOrDefaultAsync(p => p.PostmanId == request.PostmanId && p.IsActive);
            
            var package = await _context.Packages.FindAsync(request.TrackingGuid);
            
            // Validate the postman and check if their home warehouse exists or not
            if (postman == null || !postman.IsActive || postman.Warehouse == null)
            {
                _logger.LogWarning("Transfer failed: Active postman {PostmanId} not found.", request.PostmanId);
                return new ServiceResult(false, "Active postman not found.");
            }

            // Validate if the postman's warehouse is active or not
            if (!postman.Warehouse.IsActive)
            {
                _logger.LogWarning("Pickup failed: Warehouse {WarehouseName} is currently under maintanance or is decommissioned.", postman.Warehouse.WarehouseName);
                return new ServiceResult(false, "The assigned warehouse is currently inactive.");
            }
            
            
            // Check if the package exists
            if (package == null)
            {
                _logger.LogWarning("Transfer failed: Package {Guid} not found.", request.TrackingGuid);
                return new ServiceResult(false, "Package not found.");
            }

            // Make sure the package was not already picked up!
            if (package.CalculatedStatus != PackageStatus.InTransit)
            {
                _logger.LogWarning("Transfer failed: Package {Guid} is not currently In Transit.", request.TrackingGuid);
                return new ServiceResult(false, "Package status doesn't allow transfer to a new warehouse.");
            }

            var warehouse = await _context.Warehouses.FindAsync(postman.WarehouseId);
            // Validate the warehouse 
            if (warehouse == null || !warehouse.IsActive)
            {
                _logger.LogWarning("Transfer failed: Destination warehouse {WarehouseId} not found.", postman.WarehouseId);
                return new ServiceResult(false, "Warehouse not found.");
            }

            var lastEvent = await _context.Events
                .Include(e => e.EventType)
                .Include(e => e.Warehouse)
                .Where(e => e.PackageId == package.TrackingGuid)
                .OrderByDescending(e => e.Timestamp)
                .FirstOrDefaultAsync();

            // Make sure the package wasn't delivered
            if (lastEvent?.EventType.Code == EventTypeCode.Delivered)
            {
                _logger.LogWarning("Package Transfer failed: Package {Guid} was already delivered.", request.TrackingGuid);
                return new ServiceResult(false, "Package is already delivered.");
            }
            
            // Make sure the package was previously picked up
            if (lastEvent == null || lastEvent.EventType.Code != EventTypeCode.PickedUp)
            {
                _logger.LogWarning("Transfer failed: Package {Guid}'s last event was not 'Picked Up'.", request.TrackingGuid);
                return new ServiceResult(false, $"Last package status is not '{EventTypeCode.PickedUp}'.");
            }

            // Check if the current warehouse is in the same city as the package's destination city! Packages may
            // not be transferred to a different warehouse in a different city once they reach the destination city!
            if (lastEvent.Warehouse.CityId == package.FinalDestinationCityId && postman.Warehouse.CityId != package.FinalDestinationCityId)
            {
                _logger.LogWarning("Package Transfer failed: Package {Guid} was already in the final destination city! You cannot transfer it to another warehouse in a different city.", request.TrackingGuid);
                return new ServiceResult(false, "Package may not leave destination city once it arrives there.");
            }

            var warehouseEntryType = await _context.EventTypes.FirstAsync(et => et.Code == EventTypeCode.InWarehouse);

            var transferEvent = new Event
            {
                PackageId = package.TrackingGuid,
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
            var postman = await _context.Postmen
                .Include(p => p.Warehouse)
                .FirstOrDefaultAsync(p => p.PostmanId == request.PostmanId && p.IsActive);

            var package = await _context.Packages.FindAsync(request.TrackingGuid);
            
            // Validate the postman, whether he's active, and if his assigned warehouse is active!
            if (postman == null || !postman.IsActive || postman.Warehouse == null)
            {
                _logger.LogWarning("Package Delivery failed: Active postman {PostmanId} not found.", request.PostmanId);
                return new ServiceResult(false, "Postman not found.");
            }

            // Validate the package
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

            // Make sure the package had a previous event
            if (lastEvent == null)
            {
                _logger.LogWarning("Package Delivery failed: No event history for {Guid}.", request.TrackingGuid);
                return new ServiceResult(false, "Package history not found.");
            }

            // Make sure the package was not delivered earlier!
            if (lastEvent.EventTypeCode == EventTypeCode.Delivered)
            {
                _logger.LogWarning("Package Delivery failed: Package {Guid} was already delivered.", request.TrackingGuid);
                return new ServiceResult(false, "Package is already delivered.");
            }

            // Check if the last warehouse is valid, and the package was previously stored in the final destination 
            // city's warehouse. Package may only be delivered to customers from the destination city warehouse! 
            if (lastEvent.Warehouse == null || package.FinalDestinationCityId != lastEvent.Warehouse.CityId)
            {
                _logger.LogWarning("Package Delivery failed: Postman is not authorized to deliver {Guid} in cities they are not assigned to.", request.TrackingGuid);
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
            var postman = await _context.Postmen
                .Include(p => p.Warehouse)
                .FirstOrDefaultAsync(p => p.PostmanId == request.PostmanId && p.IsActive);

            var package = await _context.Packages.FindAsync(request.TrackingGuid);

            // Validate the postman, if they are active, and if their assigned warehouse is active
            if (postman == null || !postman.IsActive || postman.Warehouse == null)
            {
                _logger.LogWarning("Package Cancellation failed: Active postman {PostmanId} not found.", request.PostmanId);
                return new ServiceResult(false, "Active postman not found.");
            }

            // Validate the package itself
            if (package == null)
            {
                _logger.LogWarning("Package Cancellation failed: Package {Guid} not found.", request.TrackingGuid);
                return new ServiceResult(false, "Package not found.");
            }

            // Make sure the package was not canceled already!
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

            // Make sure there were previous events
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

        public async Task<ServiceResult<List<EventTrackingDto>>> GetPackageHistoryAsync(Guid trackingGuid)
        {
            var package = await _context.Packages.FindAsync(trackingGuid);

            // Verify the package
            if (package == null)
            {
                return new ServiceResult<List<EventTrackingDto>>(false, "Package not found.");
            }

            var events = await _context.Events
                .Include(e => e.EventType)
                .Include(e => e.Postman)
                .Include(e => e.Warehouse)
                .Where(e => e.PackageId == trackingGuid)
                .OrderBy(e => e.Timestamp)
                .Select(e => new EventTrackingDto(
                    e.EventId,
                    e.EventTypeId,
                    e.EventType.EventTypeName,
                    e.Postman != null ? e.Postman.Name : "N/A",
                    e.Warehouse != null ? e.Warehouse.WarehouseName : "N/A",
                    e.Timestamp,
                    e.Details!
                ))
                .ToListAsync();

            return new ServiceResult<List<EventTrackingDto>>(true, "History retrieved successfully!", events);
        }

        private string GenerateEventDetailsAsync(string eventCode, string pName, string wName)
        {
            return eventCode switch
            {
                // Persian:
                EventTypeCode.Registered  => $"بسته توسط «{pName}» در «{wName}» ثبت شد.",
                EventTypeCode.PickedUp    => $"بسته توسط «{pName}» از «{wName}» برداشته شد.",
                EventTypeCode.InWarehouse => $"بسته به «{wName}» توسط «{pName}» رسید.",
                EventTypeCode.Delivered   => $"بسته توسط «{pName}» به دست مشتری رسید (آخرین شعبه: «{wName}»).",
                EventTypeCode.Canceled    => $"بسته توسط «{pName}» لغو گردید.",
                _                         => "رویداد در سیستم ثبت گردید"

                // English (Might add language options later, that's why these are kept!):
                // EventTypeCode.Registered  => $"Package registered by {pName} at {wName}.",
                // EventTypeCode.PickedUp    => $"Picked up by postman {pName} from {wName}.",
                // EventTypeCode.InWarehouse => $"Stored at {wName} by postman {pName}.",
                // EventTypeCode.Delivered   => $"Delivered by postman {pName} (Last stop: {wName}).",
                // EventTypeCode.Canceled    => $"Package canceled by {pName}.",
                // _                         => "Event logged in system."
            };
        }
    }
}