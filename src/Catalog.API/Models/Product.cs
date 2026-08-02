namespace Catalog.API.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; private set; } = default!;
        public string? Description { get; set; } = default!;
        public decimal Price { get; set; }
        public int AvailableStock { get; set; }

        public Product(string name)
        {
            Name = name;
            Id = Guid.CreateVersion7();
        }

    }
}
