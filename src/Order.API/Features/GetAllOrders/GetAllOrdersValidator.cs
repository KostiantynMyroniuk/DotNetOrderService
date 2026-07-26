using FluentValidation;
using Order.API.Models;

namespace Order.API.Features.GetAllOrders
{
    public class GetAllOrdersValidator : AbstractValidator<GetAllOrdersQuery>
    {
        public GetAllOrdersValidator()
        {
            RuleFor(o => o.PageNumber)
                .GreaterThanOrEqualTo(1);

            RuleFor(o => o.PageSize)
                .InclusiveBetween(1, 25);

            RuleFor(o => o.OrderStatus)
                .IsInEnum();
        }
    }
}
