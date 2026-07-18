using MediatR;

namespace Order.API.Controllers
{
    public record OrderServices(
        ILogger<OrderServices> Logger,
        IMediator Mediator);
}
