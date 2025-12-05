using Microsoft.AspNetCore.Mvc;
using Shop.Contracts.Cart;
using Shop.Contracts.Catalog;
using Shop.Web.ViewModels.Cart;

namespace Shop.Web.Controllers;

public class CartController : Controller
{
    private readonly ICartClient _cartClient;
    private readonly ICatalogClient _catalogClient;

    // Por ahora simulamos un único usuario:
    private static readonly Guid DemoUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public CartController(ICartClient cartClient, ICatalogClient catalogClient)
    {
        _cartClient = cartClient;
        _catalogClient = catalogClient;
    }

    public async Task<IActionResult> Index()
    {
        var cart = await _cartClient.GetCurrentCartAsync(DemoUserId);

        var vm = new CartIndexViewModel
        {
            Items = cart.Items.ToList(),
            Total = cart.Total
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, int quantity = 1)
    {
        // Consultamos el producto para obtener título y precio
        var product = await _catalogClient.GetProductByIdAsync(productId);

        if (product is null)
        {
            return NotFound();
        }

        await _cartClient.AddItemAsync(
            DemoUserId,
            product.Id,
            product.Title,
            product.Price,
            quantity);

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int productId)
    {
        await _cartClient.RemoveItemAsync(DemoUserId, productId);
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Clear()
    {
        await _cartClient.ClearCartAsync(DemoUserId);
        return RedirectToAction("Index");
    }
}
