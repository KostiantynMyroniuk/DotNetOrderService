using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.API.Models;

namespace Order.API.Infrastructure.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<OrderModel>
    {
        public void Configure(EntityTypeBuilder<OrderModel> builder)
        {
            builder
                .HasIndex(o => o.RequestId)
                .IsUnique();

            builder.Property(o => o.RequestId)
                .IsRequired();

            builder
                .HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
