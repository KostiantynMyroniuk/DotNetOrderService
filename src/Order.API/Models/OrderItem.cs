namespace Order.API.Models
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public OrderModel Order { get; set; } = default!;
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public string ProductName { get; set; } = default!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
