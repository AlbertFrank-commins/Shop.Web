using Microsoft.AspNetCore.Mvc;
using Shop.Contracts.Orders;
using Shop.Web.ViewModels.Orders;
using Shop.Contracts.Catalog;

namespace Shop.Web.Controllers;

public class OrdersController : Controller
{
    private readonly IOrdersClient _ordersClient;
    private readonly ICatalogClient _catalogClient;
    private static readonly Guid DemoUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public OrdersController(IOrdersClient ordersClient, ICatalogClient catalogClient)
    {
        _ordersClient = ordersClient;
        _catalogClient = catalogClient; // Ahora sí existe
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

        var thumbnails = new Dictionary<int, string?>();

        foreach (var item in order.Items)
        {
            var product = await _catalogClient.GetProductByIdAsync(item.ProductId);
            if (product != null)
            {
                thumbnails[item.ProductId] = product.Thumbnail;
            }
        }

        var vm = new OrderDetailsViewModel
        {
            Order = order,
            ProductThumbnails = thumbnails
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
