using FluentValidation;

namespace Order.API.Features.CreateOrder
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderValidator()
        {
            RuleFor(o => o.RequestId)
                .NotEmpty(); 

            RuleFor(o => o.ShippingAddress)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(o => o.ShippingCity)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(o => o.ShippingCountry)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(o => o.ZipCode)
                .MaximumLength(20);

            RuleFor(o => o.Items)
                .NotEmpty();

            RuleForEach(o => o.Items)
                .SetValidator(new CreateOrderItemDtoValidator());
                
        }
    }

    public class CreateOrderItemDtoValidator : AbstractValidator<CreateOrderItemDto>
    {
        public CreateOrderItemDtoValidator()
        {
            RuleFor(i => i.ProductId)
                .NotEmpty();

            RuleFor(i => i.Quantity)
                .GreaterThan(0);
        }
    }
}
