using FluentValidation;

namespace Order.API.Features.CreateOrder
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderValidator()
        {
            RuleFor(o => o.Items);
        }
    }
}
