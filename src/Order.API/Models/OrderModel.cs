using System.ComponentModel.DataAnnotations.Schema;

namespace Order.API.Models
{
    public class OrderModel
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }

        public string ShippingAddress { get; set; } = default!;
        public string ShippingCity { get; set; } = default!;
        public string ShippingCountry { get; set; } = default!;
        public string? ZipCode { get; set; }

        public DateTime CreatedAt { get; private set; }

        public OrderStatus Status { get; private set; }

        public List<OrderItem> Items { get; set; } = new();

        [NotMapped]
        public decimal TotalAmount => Items.Sum(i => i.Quantity * i.UnitPrice);

        private OrderModel() { }

        public OrderModel(Guid requestId)
        {
            Id = Guid.CreateVersion7();
            RequestId = requestId;
            CreatedAt = DateTime.UtcNow;
            Status = OrderStatus.Processing;
        }

        public bool CanBeCancelled() => Status == OrderStatus.Processing;

        public void Cancel() => Status = OrderStatus.Cancelled;
    }

    public enum OrderStatus
    {
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }
}
