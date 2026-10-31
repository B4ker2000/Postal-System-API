using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostalSystem.Api.Models;

namespace PostalSystem.Api.Data.Configurations
{
    public class EventTypeConfiguration : IEntityTypeConfiguration<EventType>
    {
        public void Configure(EntityTypeBuilder<EventType> builder)
        {
            builder.HasIndex(e => e.Code)
                   .IsUnique();
        } 
    }
}