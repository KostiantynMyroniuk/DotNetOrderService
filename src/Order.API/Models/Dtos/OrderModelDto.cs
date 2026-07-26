namespace Order.API.Models.Dtos
{
    public class OrderModelDto
    {
        public Guid Id { get; set; }

        public string ShippingAddress { get; set; } = default!;
        public string ShippingCity { get; set; } = default!;
        public string ShippingCountry { get; set; } = default!;
        public string? ZipCode { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
