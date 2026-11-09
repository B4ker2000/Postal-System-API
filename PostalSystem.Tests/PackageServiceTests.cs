using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PostalSystem.Api.Data;
using PostalSystem.Api.Dtos;
using PostalSystem.Api.Models;
using PostalSystem.Api.Services;
using Xunit;

namespace PostalSystem.Tests
{
    public class PackageServiceTests
    {
        private readonly Mock<ILogger<PackageService>> _loggerMock = new();

        private PostalSystemDbContext GetDbContext()
        {
            // Create a unique database name for every test to keep them isolated
            var options = new DbContextOptionsBuilder<PostalSystemDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new PostalSystemDbContext(options);
        }

        private async Task SeedDatabaseAsync(PostalSystemDbContext context)
        {
            // Every test needs a little bit of data to work with
            var city = new City { CityId = 1, CityName = "Tabriz" };
            var warehouse = new Warehouse { WarehouseId = 1, WarehouseName = "Tabriz Post office", CityId = 1, IsActive = true };
            var postman = new Postman { PostmanId = 1, Name = "Siyamak", Phone = "123456", WarehouseId = 1, IsActive = true };

            context.Cities.Add(city);
            context.Warehouses.Add(warehouse);
            context.Postmen.Add(postman);

            // We also need an EventType for the 'Registered' status to work
            context.EventTypes.Add(new EventType { EventTypeId = 1, Code = "PR1", EventTypeName = "Registered" });

            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task RegisterPackageAsync_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            using var context = GetDbContext();
            await SeedDatabaseAsync(context);
            var service = new PackageService(context, _loggerMock.Object);

            var request = new RegisterPackageRequest
            {
                FinalDestinationCityId = 1, 
                DestinationAddress = "123 Main St",
                IsFlaggedAsFragile = true
            };

            // Act
            var result = await service.RegisterPackageAsync(request, 1);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            //Assert.False(string.IsNullOrEmpty(result.Data.TrackingGuid));
            //Assert.Equal(6, result.Data.DeliveryOTP.Length);
        }

        [Fact]
        public async Task RegisterPackageAsync_InactivePostman_ReturnsFailure()
        {
            // Arrange
            using var context = GetDbContext();
            await SeedDatabaseAsync(context);

            // Make the postman inactive
            var postman = await context.Postmen.FindAsync(1);
            postman!.IsActive = false;
            await context.SaveChangesAsync();

            var service = new PackageService(context, _loggerMock.Object);
            var request = new RegisterPackageRequest
            {
                FinalDestinationCityId = 1,
                DestinationAddress = "123 Main St",
                IsFlaggedAsFragile = true
            };

            // Act
            var result = await service.RegisterPackageAsync(request, 1);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Active postman not found or is not active", result.Message);
        }
    }
}