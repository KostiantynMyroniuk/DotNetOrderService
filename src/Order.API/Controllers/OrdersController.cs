using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.API.Extensions;
using Order.API.Features.CreateOrder;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderServices _orderServices;

        public OrdersController(OrderServices orderServices)
        {
            _orderServices = orderServices;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(
            [FromHeader(Name = "X-Request-Id")] Guid requestId,
            [FromBody] CreateOrderCommand command,
            CancellationToken ct)
        {
            var result = await _orderServices.Mediator.Send(command with { RequestId = requestId }, ct);

            if (result.IsSuccess)
            {
                return Created($"api/orders/{result.Value}", new { id = result.Value });
            }

            return MapFailureToResponse(result);
        }

        private IActionResult MapFailureToResponse(ResultBase result)
        {
            if (result.HasError<NotFoundError>(out var notFoundErrors))
                return NotFound(notFoundErrors.Select(e => e.Message));

            return Problem(
                detail: string.Join("; ", result.Errors.Select(e => e.Message)),
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
