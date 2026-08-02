using Catalog.API.Infrastructure;
using Catalog.API.Models;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Features.GetProductsByIds
{
    public record GetProductsByIdsQuery(IEnumerable<Guid> ProductIds) : IRequest<Result<List<ProductDto>>>;

    public class GetProductsByIdsQueryHandler : IRequestHandler<GetProductsByIdsQuery, Result<List<ProductDto>>>
    {
        private readonly CatalogDbContext _context;

        public GetProductsByIdsQueryHandler(CatalogDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<ProductDto>>> Handle(GetProductsByIdsQuery request, CancellationToken cancellationToken)
        {
            var productsByIds = await _context.Products
                .Where(p => request.ProductIds.Contains(p.Id))
                .ToListAsync(cancellationToken);

            var missingProducts = request.ProductIds.Except(productsByIds.Select(p => p.Id)).ToList();

            if (missingProducts.Any())
                return Result.Fail("Some products were not found: " + string.Join(", ", missingProducts));

            var productDtos = productsByIds
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    AvailableStock = p.AvailableStock
                }).ToList();

            return Result.Ok(productDtos);
        }
    }
}
