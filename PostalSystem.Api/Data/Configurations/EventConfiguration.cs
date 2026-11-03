using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostalSystem.Api.Models;

namespace PostalSystem.Api.Data.Configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasOne(p => p.Postman)
                   .WithMany(p => p.Events)
                   .HasForeignKey(p => p.PostmanId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent the Postmen from being deleted

            builder.HasOne(w => w.Warehouse)
                   .WithMany(w => w.Events)
                   .HasForeignKey(w => w.WarehouseId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent the Warehouses from being deleted

            builder.HasOne(p => p.Package)
                   .WithMany(p => p.Events)
                   .HasForeignKey(p => p.PackageId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent the Packages from being deleted
        }
    }
}