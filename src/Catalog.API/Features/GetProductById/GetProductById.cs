using Catalog.API.Infrastructure;
using Catalog.API.Models;
using FluentResults;
using MediatR;

namespace Catalog.API.Features.GetProductById
{
    public record GetProductByIdQuery(Guid ProductId) : IRequest<Result<ProductDto>>;

    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
    {
        private readonly CatalogDbContext _context;

        public GetProductByIdQueryHandler(CatalogDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .FindAsync(request.ProductId, cancellationToken);

            if (product is null)
                return Result.Fail($"Product with ID {request.ProductId} not found.");


            return Result.Ok(new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                AvailableStock = product.AvailableStock
            });
        }
    }
}
