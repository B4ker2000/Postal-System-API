using Microsoft.EntityFrameworkCore;
using PostalSystem.Api.Models;

namespace PostalSystem.Api.Data;

public class PostalSystemDbContext : DbContext
{
    public PostalSystemDbContext(DbContextOptions<PostalSystemDbContext> options)
        : base(options) { }

    public DbSet<City> Cities { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<Postman> Postmen { get; set; }
    public DbSet<Package> Packages { get; set; }
    public DbSet<EventType> EventTypes { get; set; }
    public DbSet<Event> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PostalSystemDbContext).Assembly);
    }
}