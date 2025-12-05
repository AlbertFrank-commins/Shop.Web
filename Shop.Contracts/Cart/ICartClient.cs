using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shop.Contracts.Cart;

public interface ICartClient
{
    Task<CartDto> GetCurrentCartAsync(Guid userId, CancellationToken ct = default);

    Task<CartDto> AddItemAsync(Guid userId, int productId, string productTitle, decimal unitPrice, int quantity, CancellationToken ct = default);

    Task<CartDto> RemoveItemAsync(Guid userId, int productId, CancellationToken ct = default);

    Task<CartDto> ClearCartAsync(Guid userId, CancellationToken ct = default);
}
