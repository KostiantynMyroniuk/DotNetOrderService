using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Events
{
    public record OrderCreatedEvent(
        Guid OrderId,
        string ShippingAddress,
        string ShippintCountry,
        string ShippingCity,
        string ZipCode,
        decimal TotalAmount,
        DateTime CreatedAt,
        List<OrderItemDto> Items);

    public record OrderItemDto(
        Guid ProductId,
        string ProductName,
        int Quantity,
        decimal UnitPrice);
}
