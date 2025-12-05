using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shop.Contracts.Orders;

public interface IOrdersClient
{
    Task<OrderDto> CreateFromCartAsync(Guid userId, CancellationToken ct = default);

    Task<IReadOnlyList<OrderDto>> GetOrdersForUserAsync(Guid userId, CancellationToken ct = default);

    Task<OrderDto?> GetByIdAsync(Guid orderId, CancellationToken ct = default);

    Task UpdateStatusAsync(Guid orderId, OrderStatus newStatus, CancellationToken ct = default);
}
