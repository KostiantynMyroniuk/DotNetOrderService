using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;

namespace Order.API.Infrastructure
{
    public class OrderDbContext : DbContext
    {
        public DbSet<OrderModel> Orders { get; set; } = default!;

        public DbSet<OrderItem> OrderItems { get; set; } = default!;

        public DbSet<Product> Products { get; set; } = default!;

        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Program).Assembly);

            modelBuilder.AddTransactionalOutboxEntities();
        }
    }
}
