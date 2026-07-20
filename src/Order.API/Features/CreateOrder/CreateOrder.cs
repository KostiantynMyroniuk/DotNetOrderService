using FluentResults;
using MassTransit;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Order.API.Extensions;
using Order.API.Infrastructure;
using Order.API.Models;
using Shared.Events;

namespace Order.API.Features.CreateOrder
{

    public record CreateOrderItemDto(Guid ProductId, int Quantity);
    public record CreateOrderCommand(
        Guid RequestId,
        string ShippingAddress,
        string ShippingCity,
        string ShippingCountry,
        string? ZipCode,
        List<CreateOrderItemDto> Items
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
            var order = new OrderModel(request.RequestId)
            {
                ShippingAddress = request.ShippingAddress,
                ShippingCity = request.ShippingCity,
                ShippingCountry = request.ShippingCountry,
                ZipCode = request.ZipCode
            };

            var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();

            //while products in the same dbcontext (later can be used grpc/events to take info about products)
            var products = await _context.Products
                .AsNoTracking()
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync(cancellationToken);

            var productsById = products.ToDictionary(p => p.Id);

            var missingIds = productIds.Except(productsById.Keys).ToList();
            if (missingIds.Count > 0)
                return Result.Fail(new NotFoundError($"Products is missing: {string.Join(", ", missingIds)}"));

            var items = request.Items.Select(i =>
            {
                var product = productsById[i.ProductId];

                return new OrderItem() 
                { 
                    Id = Guid.CreateVersion7(),
                    OrderId = order.Id,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = product.Price
                };

            }).ToList();

            order.Items = items;

            try
            {
                _context.Orders.Add(order);

                await _publishEndpoint.Publish(new OrderCreatedEvent(
                    OrderId: order.Id,
                    ShippingAddress: order.ShippingAddress,
                    ShippingCountry: order.ShippingCountry,
                    ShippingCity: order.ShippingCity,
                    ZipCode: order.ZipCode,
                    TotalAmount: order.TotalAmount,
                    CreatedAt: order.CreatedAt,
                    Items: order.Items.Select(i => new OrderItemDto(i.ProductId, i.ProductName, i.Quantity, i.UnitPrice)).ToList()), cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("OrderCreatedEvent staged in outbox for OrderId {OrderId}", order.Id);

                _logger.LogInformation("Order created successfully with requestId: {RequestId}", request.RequestId);
            }
            catch (Exception ex) when (ex.InnerException is SqlException { Number: 2601 or 2627})
            {
                _logger.LogWarning("Duplicate order request detected for requestId: {RequestId}", request.RequestId);

                _context.ChangeTracker.Clear();

                var existingOrder = await _context.Orders
                    .FirstOrDefaultAsync(o => o.RequestId == request.RequestId, cancellationToken);

                return Result.Ok(existingOrder!.Id);
            }

            return Result.Ok(order.Id);
        }
    }
}
