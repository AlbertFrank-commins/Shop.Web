using Shop.Contracts.Orders;

namespace Shop.Web.ViewModels.Orders;

public class OrderDetailsViewModel
{
    public OrderDto Order { get; init; } = null!;
}
