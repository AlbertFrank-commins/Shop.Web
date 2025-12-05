using System;
using System.Collections.Generic;
using System.Linq;

namespace Shop.Contracts.Orders;

public class OrderDto
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public Guid UserId { get; init; }

    public DateTime CreatedAt { get; init; }

    public OrderStatus Status { get; init; } = OrderStatus.Pending;

    public IReadOnlyList<OrderItemDto> Items { get; init; } = Array.Empty<OrderItemDto>();

    public decimal TotalAmount => Items.Sum(i => i.UnitPrice * i.Quantity);
}
