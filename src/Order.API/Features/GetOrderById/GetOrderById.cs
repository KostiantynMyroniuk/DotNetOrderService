using FluentResults;
using MediatR;
using Order.API.Extensions;
using Order.API.Infrastructure;
using Order.API.Models.Dtos;

namespace Order.API.Features.GetOrderById
{
    public record GetOrderByIdQuery(
        Guid OrderId) : IRequest<Result<OrderModelDto>>;

    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderModelDto>>
    {
        private readonly OrderDbContext _context;

        public GetOrderByIdQueryHandler(
            OrderDbContext context)
        {
            _context = context;
        }

        public async Task<Result<OrderModelDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders
                .FindAsync(request.OrderId, cancellationToken);

            if (order is null)
                return Result.Fail(new NotFoundError($"Order {request.OrderId} is missing"));

            var orderDto = new OrderModelDto()
            {
                Id = order.Id,
                ShippingAddress = order.ShippingAddress,
                ShippingCity = order.ShippingCity,
                ShippingCountry = order.ShippingCountry,
                CreatedAt = order.CreatedAt,
            };

            return Result.Ok(orderDto);
        }
    }
}
