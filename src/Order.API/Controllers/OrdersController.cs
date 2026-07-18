using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.API.Features.Commands;

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
            [FromBody] CreateOrderCommand command)
        {
            var result = await _orderServices.Mediator.Send(command with { requestId = requestId }, cancellationToken: default);

            return result.IsSuccess 
                ? CreatedAtAction(nameof(/* Get later */ CreateOrder), new { id = result.Value }, result.Value)
                : Conflict(result.Errors);
        }
    }
}
