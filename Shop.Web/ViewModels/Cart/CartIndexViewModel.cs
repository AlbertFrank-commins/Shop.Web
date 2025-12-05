using System.Collections.Generic;
using Shop.Contracts.Cart;

namespace Shop.Web.ViewModels.Cart;

public class CartIndexViewModel
{
    public List<CartItemDto> Items { get; init; } = new();
    public decimal Total { get; init; }
    // ProductId -> URL de miniatura
    public Dictionary<int, string?> ProductThumbnails { get; init; } = new();
}
