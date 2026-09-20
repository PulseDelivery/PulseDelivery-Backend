using Microsoft.EntityFrameworkCore;
using Order.Domain.Entities;

namespace Order.Infrastructure.Persistence;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<OrderEntity> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Entity configuration
        modelBuilder.Entity<OrderEntity>()
            .Ignore("State"); // The State interface is not persisted to the database; it only exists at runtime.

        base.OnModelCreating(modelBuilder);
    }
}