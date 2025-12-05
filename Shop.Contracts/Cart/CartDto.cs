using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Contracts.Cart
{
    public class CartDto
    {
        public Guid CartId { get; init; } = Guid.NewGuid();
        public Guid UserId { get; init; }
        public IReadOnlyList<CartItemDto> Items { get; init; } = Array.Empty<CartItemDto>();

        public decimal Total => Items.Sum(i => i.UnitPrice * i.Quantity);
    }
}
