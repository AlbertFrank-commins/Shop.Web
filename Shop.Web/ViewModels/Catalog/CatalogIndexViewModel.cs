using System;
using System.Collections.Generic;
using Shop.Contracts.Catalog;

namespace Shop.Web.ViewModels.Catalog;

public class CatalogIndexViewModel
{
    public IReadOnlyList<ProductDto> Products { get; init; } = Array.Empty<ProductDto>();

    public int CurrentPage { get; init; }

    public int PageSize { get; init; }

    public int TotalItems { get; init; }

    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

    public string? Search { get; init; }
}
