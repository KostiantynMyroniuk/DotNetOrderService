using FluentResults;
using MassTransit;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Order.API.Infrastructure;
using Order.API.Models;
using Shared.Events;

namespace Order.API.Features.Commands
{
    public record CreateOrderCommand(
        Guid requestId,
        string ShippingAddress,
        string ShippingCity,
        string ShippingCountry,
        string? ZipCode,
        List<OrderItem> Items
    ) : IRequest<Result<Guid>>;

    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<Guid>>
    {
        private readonly OrderDbContext _context;
        private readonly ILogger<CreateOrderCommandHandler> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public CreateOrderCommandHandler(
            OrderDbContext context,
            ILogger<CreateOrderCommandHandler> logger,
            IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _logger = logger;
            _publishEndpoint = publishEndpoint; 
        }

        public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new OrderModel(request.requestId)
            {
                ShippingAddress = request.ShippingAddress,
                ShippingCity = request.ShippingCity,
                ShippingCountry = request.ShippingCountry,
                ZipCode = request.ZipCode,
                Items = request.Items
            };

            try
            {
                _context.Orders.Add(order);

                await _publishEndpoint.Publish(new OrderCreatedEvent(
                    OrderId: order.Id,
                    ShippingAddress: order.ShippingAddress,
                    ShippintCountry: order.ShippingCountry,
                    ShippingCity: order.ShippingCity,
                    ZipCode: order.ZipCode,
                    TotalAmount: order.TotalAmount,
                    CreatedAt: order.CreatedAt,
                    Items: order.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice)).ToList()), cancellationToken);

                _logger.LogInformation("OrderCreatedEvent is published");

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Order created successfully with requestId: {RequestId}", request.requestId);
            }
            catch (Exception ex) when (ex.InnerException is SqlException { Number: 2601 or 2627})
            {
                _logger.LogWarning("Duplicate order request detected for requestId: {RequestId}", request.requestId);

                _context.ChangeTracker.Clear();

                var existingOrder = await _context.Orders
                    .FirstOrDefaultAsync(o => o.RequestId == request.requestId, cancellationToken);

                return Result.Ok(existingOrder!.Id);
            }

            return Result.Ok(order.Id);
        }
    }
}
