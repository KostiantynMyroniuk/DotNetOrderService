namespace Catalog.API.Models
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; } = default!;
        public decimal Price { get; set; }
        public int AvailableStock { get; set; }
    }
}
