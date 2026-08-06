using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.API.Extensions;
using Order.API.Features.CancelOrder;
using Order.API.Features.CreateOrder;
using Order.API.Features.GetAllOrders;
using Order.API.Features.GetOrderById;
using Order.API.Models.Dtos;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(
            [FromHeader(Name = "X-Request-Id")] Guid requestId,
            [FromBody] CreateOrderCommand command,
            CancellationToken ct)
        {
            var result = await _mediator.Send(command with { RequestId = requestId }, ct);

            if (result.IsFailed)
                return result.ToActionResult(this);

            return Created($"api/orders/{result.Value}", result.Value);
        }

        [HttpGet("{orderId:guid}")]
        public async Task<ActionResult<OrderModelDto>> GetOrderById(
            [FromRoute] Guid orderId,
            CancellationToken ct)
        {
            var result = await _mediator.Send(new GetOrderByIdQuery(orderId), ct);

            if (result.IsFailed)
                return result.ToActionResult(this);

            return Ok(result.Value);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllOrders(
            [FromQuery] GetAllOrdersQuery query,
            CancellationToken ct)
        {
            var items = await _mediator.Send(query, ct);

            return Ok(items);
        }

        [HttpPost("{orderId:guid}/cancel")]
        public async Task<IActionResult> CancelOrder([FromRoute] Guid orderId, CancellationToken ct)
        {
            var result = await _mediator.Send(new CancelOrderCommand(orderId), ct);
            
            return result.IsSuccess ? NoContent() : result.ToActionResult(this);
        }

    }
}
