using System.Collections.Generic;
using Shop.Contracts.Orders;

namespace Shop.Web.ViewModels.Orders;

public class OrdersListViewModel
{
    public List<OrderDto> Orders { get; init; } = new();
    // ProductId -> URL de la miniatura
 
}
