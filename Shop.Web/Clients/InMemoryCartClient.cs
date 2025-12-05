using Shop.Contracts.Cart;

namespace Shop.Web.Clients;

public class InMemoryCartClient : ICartClient
{
    // Carritos en memoria por usuario
    private static readonly Dictionary<Guid, CartDto> _carts = new();

    private static readonly object _lock = new();

    public Task<CartDto> GetCurrentCartAsync(Guid userId, CancellationToken ct = default)
    {
        lock (_lock)
        {
            if (!_carts.TryGetValue(userId, out var cart))
            {
                cart = new CartDto
                {
                    UserId = userId,
                    Items = Array.Empty<CartItemDto>()
                };
                _carts[userId] = cart;
            }

            return Task.FromResult(cart);
        }
    }

    public Task<CartDto> AddItemAsync(Guid userId, int productId, string productTitle, decimal unitPrice, int quantity, CancellationToken ct = default)
    {
        lock (_lock)
        {
            if (!_carts.TryGetValue(userId, out var cart))
            {
                cart = new CartDto
                {
                    UserId = userId,
                    Items = Array.Empty<CartItemDto>()
                };
            }

            var items = cart.Items.ToList();
            var existing = items.FirstOrDefault(i => i.ProductId == productId);

            if (existing is null)
            {
                items.Add(new CartItemDto(productId, productTitle, unitPrice, quantity));
            }
            else
            {
                items.Remove(existing);
                items.Add(existing with { Quantity = existing.Quantity + quantity });
            }

            cart = new CartDto
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                Items = items
            };

            _carts[userId] = cart;

            return Task.FromResult(cart);
        }
    }

    public Task<CartDto> RemoveItemAsync(Guid userId, int productId, CancellationToken ct = default)
    {
        lock (_lock)
        {
            if (!_carts.TryGetValue(userId, out var cart))
            {
                return Task.FromResult(new CartDto { UserId = userId });
            }

            var items = cart.Items.Where(i => i.ProductId != productId).ToList();

            cart = new CartDto
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                Items = items
            };

            _carts[userId] = cart;
            return Task.FromResult(cart);
        }
    }

    public Task<CartDto> ClearCartAsync(Guid userId, CancellationToken ct = default)
    {
        lock (_lock)
        {
            var emptyCart = new CartDto
            {
                UserId = userId,
                Items = Array.Empty<CartItemDto>()
            };
            _carts[userId] = emptyCart;
            return Task.FromResult(emptyCart);
        }
    }
}
