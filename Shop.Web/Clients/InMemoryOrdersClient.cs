using Shop.Contracts.Cart;
using Shop.Contracts.Orders;

namespace Shop.Web.Clients;

public class InMemoryOrdersClient : IOrdersClient
{
    private readonly ICartClient _cartClient;

    private static readonly List<OrderDto> _orders = new();
    private static readonly object _lock = new();

    public InMemoryOrdersClient(ICartClient cartClient)
    {
        _cartClient = cartClient;
    }

    public async Task<OrderDto> CreateFromCartAsync(Guid userId, CancellationToken ct = default)
    {
        // Tomamos el carrito actual
        var cart = await _cartClient.GetCurrentCartAsync(userId, ct);

        if (!cart.Items.Any())
        {
            throw new InvalidOperationException("El carrito está vacío, no se puede crear la orden.");
        }

        var items = cart.Items
            .Select(i => new OrderItemDto(
                i.ProductId,
                i.ProductTitle,
                i.UnitPrice,
                i.Quantity))
            .ToList();

        var order = new OrderDto
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            Items = items
        };

        lock (_lock)
        {
            _orders.Add(order);
        }

        // Limpiamos el carrito después de crear la orden
        await _cartClient.ClearCartAsync(userId, ct);

        return order;
    }

    public Task<IReadOnlyList<OrderDto>> GetOrdersForUserAsync(Guid userId, CancellationToken ct = default)
    {
        lock (_lock)
        {
            var userOrders = _orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToList()
                .AsReadOnly();

            return Task.FromResult((IReadOnlyList<OrderDto>)userOrders);
        }
    }

    public Task<OrderDto?> GetByIdAsync(Guid orderId, CancellationToken ct = default)
    {
        lock (_lock)
        {
            var order = _orders.FirstOrDefault(o => o.Id == orderId);
            return Task.FromResult(order);
        }
    }
    public Task UpdateStatusAsync(Guid orderId, OrderStatus newStatus, CancellationToken ct = default)
    {
        lock (_lock)
        {
            var existing = _orders.FirstOrDefault(o => o.Id == orderId);
            if (existing is null)
            {
                return Task.CompletedTask;
            }

            var updated = new OrderDto
            {
                Id = existing.Id,
                UserId = existing.UserId,
                CreatedAt = existing.CreatedAt,
                Status = newStatus,
                Items = existing.Items
            };

            _orders.Remove(existing);
            _orders.Add(updated);

            return Task.CompletedTask;
        }
    }

}
