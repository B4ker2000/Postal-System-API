using System.Security.Authentication;
using Microsoft.EntityFrameworkCore;
using PostalSystem.Api.Date;
using PostalSystem.Api.Models;

namespace PostalSystem.Api.Data;

public class PostalSystemDbContext : DbContext
{
    public PostalSystemDbContext(DbContextOptions<PostalSystemDbContext>options)
        : base(options){}

    public DbSet<City> Cities { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<Postman> Postmen { get; set; }
    public DbSet<Package> Packages { get; set; }
    public DbSet<EventType> EventTypes { get; set; }
    public DbSet<Event> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Postman>()
            .HasIndex(p => p.Phone)
            .IsUnique();

        modelBuilder.Entity<Package>()
            .HasIndex(p => p.TrackingGuid)
            .IsUnique();
            
        modelBuilder.Entity<EventType>()
            .HasIndex(e => e.EventTypeName)
            .IsUnique();

        modelBuilder.Entity<Package>()
            .HasOne(p => p.OriginWarehouse)
            .WithMany(w => w.PackagesOrigin)
            .HasForeignKey(p => p.OriginWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Package>()
            .HasOne(p => p.CurrentWarehouse)
            .WithMany(w => w.PackagesCurrent)
            .HasForeignKey(p => p.CurrentWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Package>()
            .HasOne(p => p.FinalDestinationWarehouse)
            .WithMany(w => w.PackagesDestination)
            .HasForeignKey(p => p.FinalDestinationWarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}