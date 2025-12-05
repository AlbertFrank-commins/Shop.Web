using Microsoft.AspNetCore.Mvc;
using Shop.Contracts.Orders;
using Shop.Web.ViewModels.Orders;

namespace Shop.Web.Controllers;

public class OrdersController : Controller
{
    private readonly IOrdersClient _ordersClient;

    private static readonly Guid DemoUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public OrdersController(IOrdersClient ordersClient)
    {
        _ordersClient = ordersClient;
    }

    // GET: /Orders
    public async Task<IActionResult> Index()
    {
        var orders = await _ordersClient.GetOrdersForUserAsync(DemoUserId);

        // 🔴 AQUÍ USAMOS OrdersListViewModel, NO OrderDetailsViewModel
        var vm = new OrdersListViewModel
        {
            Orders = orders.ToList()
        };

        return View(vm);
    }

    // GET: /Orders/Details/{id}
    public async Task<IActionResult> Details(Guid id)
    {
        var order = await _ordersClient.GetByIdAsync(id);

        if (order is null)
        {
            return NotFound();
        }

        var vm = new OrderDetailsViewModel
        {
            Order = order
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout()
    {
        try
        {
            var order = await _ordersClient.CreateFromCartAsync(DemoUserId);
            return RedirectToAction("Details", new { id = order.Id });
        }
        catch (InvalidOperationException)
        {
            return RedirectToAction("Index", "Cart");
        }
    }
}
