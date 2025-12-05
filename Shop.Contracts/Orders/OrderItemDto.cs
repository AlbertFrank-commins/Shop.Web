using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Contracts.Orders
{
    public record OrderItemDto(
    int ProductId,
    string ProductTitle,
    decimal UnitPrice,
    int Quantity);
}
