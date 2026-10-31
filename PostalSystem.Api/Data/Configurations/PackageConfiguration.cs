using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostalSystem.Api.Models;

namespace PostalSystem.Api.Data.Configurations
{
    public class PackageConfiguration : IEntityTypeConfiguration<Package>
    {
        public void Configure(EntityTypeBuilder<Package> builder)
        {
            builder.HasKey(p => p.TrackingGuid);

            builder.HasOne(p => p.FinalDestinationCity)
                   .WithMany()
                   .HasForeignKey(p => p.FinalDestinationCityId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}