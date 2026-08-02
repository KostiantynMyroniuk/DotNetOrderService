using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.API.Infrastructure.Configurations
{
    public class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(p => p.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(p => p.AvailableStock)
                .IsRequired();

            builder.HasData(
                new Product("Product 1")
                {
                    Description = "Description for Product 1",
                    Price = 10.99m,
                    AvailableStock = 5
                },

                new Product("Product 2")
                {
                    Description = "Description for Product 2",
                    Price = 19.99m,
                    AvailableStock = 19
                },

                new Product("Product 3")
                {
                    Description = "Description for Product 3",
                    Price = 5.99m,
                    AvailableStock = 0
                }
            );
        }
    }
}
