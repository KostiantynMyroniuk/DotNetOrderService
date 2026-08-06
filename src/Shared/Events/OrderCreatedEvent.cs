using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Events
{
    public record OrderCreatedEvent(
        Guid OrderId,
        ShippingAddressDto ShippingAddress,
        decimal TotalAmount,
        DateTime CreatedAt,
        List<OrderItemDto> Items);

    public record ShippingAddressDto(
        string ShippingAddress,
        string ShippingCountry,
        string ShippingCity,
        string? ZipCode);

    public record OrderItemDto(
        Guid ProductId,
        string ProductName,
        int Quantity,
        decimal UnitPrice);
}
