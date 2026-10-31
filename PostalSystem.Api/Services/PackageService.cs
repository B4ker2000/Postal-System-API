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
    }
}