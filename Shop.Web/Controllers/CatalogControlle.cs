using Microsoft.AspNetCore.Mvc;
using Shop.Contracts.Catalog;
using Shop.Web.ViewModels.Catalog;

namespace Shop.Web.Controllers;

public class CatalogController : Controller
{
    private readonly ICatalogClient _catalogClient;

    public CatalogController(ICatalogClient catalogClient)
    {
        _catalogClient = catalogClient;
    }

    // GET: /Catalog
    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        const int pageSize = 12;

        if (page < 1)
        {
            page = 1;
        }

        int skip = (page - 1) * pageSize;

        var productsResult = await _catalogClient.GetProductsAsync(skip, pageSize, search);

        var vm = new CatalogIndexViewModel
        {
            Products = productsResult.Items,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = productsResult.Total,
            Search = search
        };


        return View("~/Views/Catalog/Index.cshtml", vm);

    }


    public async Task<IActionResult> Details(int id)
    {
        var product = await _catalogClient.GetProductByIdAsync(id);

        if (product is null)
        {
            return NotFound();
        }

        return View(product);
    }
}
