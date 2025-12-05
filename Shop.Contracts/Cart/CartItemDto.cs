using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Contracts.Cart
{
    public record CartItemDto(
    int ProductId,
    string ProductTitle,
    decimal UnitPrice,
    int Quantity);
}
