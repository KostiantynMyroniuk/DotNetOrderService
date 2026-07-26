using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.API.Infrastructure;
using Order.API.Models;
using Order.API.Models.Dtos;
using Shared.Models;

namespace Order.API.Features.GetAllOrders
{
    public record GetAllOrdersQuery(
        int PageNumber,
        int PageSize,
        OrderStatus? OrderStatus
        ) : IRequest<PaginatedList<OrderModelDto>>;

    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, PaginatedList<OrderModelDto>>
    {
        private readonly OrderDbContext _context;

        public GetAllOrdersQueryHandler(
            OrderDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<OrderModelDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Orders
                .AsNoTracking();

            if (request.OrderStatus is not null)
                query = query.Where(o => o.Status == request.OrderStatus);

            var totalCount = await query.CountAsync(cancellationToken);

            var orderDtoList = await query
                .OrderByDescending(o => o.CreatedAt).ThenBy(o => o.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(o => new OrderModelDto
                {
                    Id = o.Id,
                    ShippingAddress = o.ShippingAddress,
                    ShippingCity = o.ShippingCity,
                    ShippingCountry = o.ShippingCountry,
                    ZipCode = o.ZipCode,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                })
                .ToListAsync(cancellationToken);

            return new PaginatedList<OrderModelDto>(orderDtoList, request.PageNumber, request.PageSize, totalCount);  
        }
    }
}
