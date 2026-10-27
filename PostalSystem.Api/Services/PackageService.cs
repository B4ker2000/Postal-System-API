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
            // 1. Find the logged-in postman and their assigned warehouse
            var postman = _context.Postmen
                .FirstOrDefault(p => p.PostmanId == postmanId && p.IsActive);

            if (postman == null)
            {
                throw new ArgumentException("Active postman not found.");
            }

            // 2. Generate random tracking GUID and a 6-digit OTP code
            var trackingGuid = Guid.NewGuid();
            var deliveryOtp = new Random().Next(100000, 999999).ToString();

            // 3. Construct the package entity using business constraints
            var package = new Package
            {
                TrackingGuid = trackingGuid,
                DeliveryOTP = deliveryOtp,
                FinalDestinationCityId = dto.FinalDestinationCityId,
                DestinationAddress = dto.DestinationAddress,
                IsFlaggedAsFragile = dto.IsFlaggedAsFragile,
                CalculatedStatus = PackageStatus.Registered,
                IsCanceled = false
            };

            // 4. Save to the database
            _context.Packages.Add(package);
            await _context.SaveChangesAsync();

            // 5. TODO: Add Event logging here

            return package;
        }
    }
}