using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PostalSystem.Api.Models;

namespace PostalSystem.Api.Data.Configurations
{
    public class PostmanConfiguration : IEntityTypeConfiguration<Postman>
    {
        public void Configure(EntityTypeBuilder<Postman> builder)
        {
            builder.HasIndex(p => p.Phone)
                   .IsUnique();
        }
    }
}