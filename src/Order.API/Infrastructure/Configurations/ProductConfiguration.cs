using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.API.Models;

namespace Order.API.Infrastructure.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.HasData(
                new Product() 
                {
                    Id = Guid.Parse("018f4e2a-7b91-7c3d-8a2f-000000000001"),
                    Name = "Laptop Asus TUF Gaming A15",
                    Description = "Laptop for work, gaming, etc.",
                    Price = 44299.00M
                },

                new Product()
                {
                    Id = Guid.Parse("018f4e2a-7b91-7c3d-8a2f-000000000002"),
                    Name = "Apple Iphone 17 Pro",
                    Description = "",
                    Price = 39999.00M
                }
            );
        }
    }
}
