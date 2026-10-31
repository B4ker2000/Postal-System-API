using Microsoft.EntityFrameworkCore;
using PostalSystem.Api.Data;
using PostalSystem.Api.Dtos;
using PostalSystem.Api.Models;

namespace PostalSystem.Api.Services
{
    public class PackageService : IPackageService
    {
        private readonly PostalSystemDbContext _context;

        public PackageService(PostalSystemDbContext context)
        {
            _context = context;
        }

        public async Task<Package> RegisterPackageAsync(int postmanId, RegisterPackageDto dto)
        {
            // 1. Validate the postman and get their home warehouse
            var postman = await _context.Postmen
                .FirstOrDefaultAsync(p => p.PostmanId == postmanId && p.IsActive);

            if (postman == null)
                throw new ArgumentException("Active postman not found.");

            // 2. Create the Package entity
            // Also generate a random tracking GUID and a 6-digit OTP code
            var package = new Package
            {
                TrackingGuid = Guid.NewGuid(),
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
                .FirstOrDefaultAsync(et => et.Code == "NP1");

            if (registeredEventType == null)
                throw new InvalidOperationException("Event type 'NP1' not found in database.");

            var registrationEvent = new Event
            {
                Package = package,
                EventType = registeredEventType,
                PostmanId = postmanId,
                WarehouseId = postman.WarehouseId, // This marks the Origin!
                Timestamp = DateTime.UtcNow,
                Details = "Package registered into the system." // take from eventType desc perhaps?
            };

            _context.Events.Add(registrationEvent);

            // 4. Save everything in one transaction
            await _context.SaveChangesAsync();

            return package;
        }

        public async Task<Package> PickupPackageAsync(int postmanId, Guid trackingGuid)
        {
            // 1. Find the postman and their warehouse
            var postman = await _context.Postmen
                .FirstOrDefaultAsync(p => p.PostmanId == postmanId && p.IsActive);

            if (postman == null) throw new ArgumentException("Active postman not found.");

            // 2. Find the package and its entire event history (sorted by newest first)
            var package = await _context.Packages
                .Include(p => p.Events.OrderByDescending(e => e.Timestamp))
                .FirstOrDefaultAsync(p => p.TrackingGuid == trackingGuid);

            if (package == null) throw new ArgumentException("Package not found.");
            if (package.IsCanceled) throw new InvalidOperationException("Cannot pick up a canceled package.");

            // 3. Determine current location from the latest event
            var lastEvent = package.Events.FirstOrDefault();
            if (lastEvent == null) throw new InvalidOperationException("Package has no history.");

            // Validation: The last event MUST be an 'Entry' (Category 1) and at the postman's warehouse
             if (lastEvent.EventType.Category != EventCategory.Entry)
                 throw new InvalidOperationException("Package is not currently in a warehouse.");

            if (lastEvent.WarehouseId != postman.WarehouseId)
                throw new InvalidOperationException("This package is not at your assigned warehouse!");

            // 4. Create the "Picked Up" Event (the "Exit" stamp)
            var pickupEventType = await _context.EventTypes
                .FirstOrDefaultAsync(et => et.Code == "PP2");
            
            if (pickupEventType == null)
                throw new InvalidOperationException("Event type 'PP2' not found.");

            var pickupEvent = new Event
            {
                PackageId = package.TrackingGuid,
                EventType = pickupEventType,
                PostmanId = postmanId,
                WarehouseId = postman.WarehouseId,
                Timestamp = DateTime.UtcNow,
                Details = $"Picked up by postman {postman.Name}."
            };

            // 5. Update Package state
            package.CalculatedStatus = PackageStatus.InTransit;
            _context.Events.Add(pickupEvent);

            await _context.SaveChangesAsync();

            return package;
        }

        public async Task<bool> ConfirmDeliveryAsync(ConfirmDeliveryRequest request)
        {
            var package = await _context.Packages
                .FirstOrDefaultAsync(p => p.TrackingGuid == request.TrackingGuid);

            if (package == null || package.IsCanceled)
                return false;

            if (package.DeliveryOTP != request.Otp)
                return false;

            // Ensure the package is actually out for delivery
            var lastEvent = await _context.Events
                .Where(e => e.PackageId == package.TrackingGuid)
                .OrderByDescending(e => e.Timestamp)
                .FirstOrDefaultAsync();

            if (lastEvent == null || lastEvent.EventType.Code != "PP2")
                return false;

            var deliveryEventType = await _context.EventTypes
                .FirstOrDefaultAsync(et => et.Code == "PP2");

            if (deliveryEventType == null)
                throw new Exception("Delivery event type 'PP2' not configured in database.");

            var deliveryEvent = new Event
            {
                PackageId = package.TrackingGuid,
                EventTypeId = deliveryEventType.EventTypeId,
                Timestamp = DateTime.UtcNow,
                Details = "Package delivered successfully."
            };

            package.CalculatedStatus = PackageStatus.Delivered;

            _context.Events.Add(deliveryEvent);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}