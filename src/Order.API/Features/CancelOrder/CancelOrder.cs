using FluentResults;
using MassTransit;
using MediatR;
using Order.API.Extensions;
using Order.API.Infrastructure;
using Order.API.Models;
using Shared.Events;

namespace Order.API.Features.CancelOrder
{
    public record CancelOrderCommand(Guid OrderId) : IRequest<Result>;

    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly OrderDbContext _context;
        private readonly ILogger<CancelOrderCommandHandler> _logger;

        public CancelOrderCommandHandler(
            IPublishEndpoint publishEndpoint,
            OrderDbContext context,
            ILogger<CancelOrderCommandHandler> logger)
        {
            _publishEndpoint = publishEndpoint;
            _context = context;
            _logger = logger;
        }

        public async Task<Result> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Cancelling order {OrderId}", request.OrderId);

            var order = await _context.Orders.FindAsync([request.OrderId], cancellationToken);

            if (order is null)
            {
                _logger.LogWarning("Order {OrderId} not found", request.OrderId);

                return Result.Fail(new NotFoundError($"Order {request.OrderId} not found"));
            }

            if (!order.CanBeCancelled())
            {
                _logger.LogWarning("Order {OrderId} cannot be cancelled. Current status: {Status}",
                    request.OrderId, order.Status);

                return Result.Fail($"Order {request.OrderId} cannot be cancelled");
            }

            order.Cancel();

            await _publishEndpoint.Publish(
                new OrderCancelledEvent(order.Id, DateTime.UtcNow),
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Order {OrderId} cancelled successfully", order.Id);

            return Result.Ok();
        }
    }
}
