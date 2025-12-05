using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Shop.BuildingBlocks.Pagination;
using Shop.Contracts.Catalog;

namespace Shop.Web.Clients;

public class DummyJsonCatalogClient : ICatalogClient
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public DummyJsonCatalogClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PagedResult<ProductDto>> GetProductsAsync(
        int skip = 0,
        int limit = 30,
        string? search = null,
        string? category = null,
        CancellationToken cancellationToken = default)
    {
        // Construir la URL según lo que soporte DummyJSON
        string url;

        if (!string.IsNullOrWhiteSpace(search))
        {
            // /products/search?q=phone&limit=30&skip=0
            url = $"/products/search?q={Uri.EscapeDataString(search)}&limit={limit}&skip={skip}";
        }
        else if (!string.IsNullOrWhiteSpace(category))
        {
            // DummyJSON tiene endpoint por categoría: /products/category/{category}
            // pero no soporta skip/limit por categoría, así que haremos algo simple
            url = $"/products/category/{Uri.EscapeDataString(category)}";
        }
        else
        {
            // /products?limit=30&skip=0
            url = $"/products?limit={limit}&skip={skip}";
        }

        var response = await _httpClient.GetAsync(url, cancellationToken);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        var apiResult = JsonSerializer.Deserialize<DummyJsonProductsResponse>(json, _jsonOptions);

        if (apiResult is null || apiResult.Products is null)
        {
            return new PagedResult<ProductDto>
            {
                Items = Array.Empty<ProductDto>(),
                Total = 0,
                Skip = skip,
                Limit = limit
            };
        }

        // Mapear a ProductDto
        var products = apiResult.Products.Select(p => new ProductDto
        {
            Id = p.Id,
            Title = p.Title,
            Description = p.Description ?? string.Empty,
            Category = p.Category ?? string.Empty,
            Brand = p.Brand ?? string.Empty,
            Price = (decimal)p.Price,
            Rating = p.Rating,
            Stock = p.Stock,
            Thumbnail = p.Thumbnail ?? string.Empty,
            Images = p.Images?.ToList() ?? new List<string>()
        }).ToList();

        return new PagedResult<ProductDto>
        {
            Items = products,
            Total = apiResult.Total,
            Skip = apiResult.Skip,
            Limit = apiResult.Limit
        };
    }

    public async Task<ProductDto?> GetProductByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/products/{id}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        var apiProduct = JsonSerializer.Deserialize<DummyJsonProduct>(json, _jsonOptions);

        if (apiProduct is null)
        {
            return null;
        }

        return new ProductDto
        {
            Id = apiProduct.Id,
            Title = apiProduct.Title,
            Description = apiProduct.Description ?? string.Empty,
            Category = apiProduct.Category ?? string.Empty,
            Brand = apiProduct.Brand ?? string.Empty,
            Price = (decimal)apiProduct.Price,
            Rating = apiProduct.Rating,
            Stock = apiProduct.Stock,
            Thumbnail = apiProduct.Thumbnail ?? string.Empty,
            Images = apiProduct.Images?.ToList() ?? new List<string>()
        };
    }

    // Clases privadas internas para mapear la respuesta cruda de DummyJSON
    private class DummyJsonProductsResponse
    {
        public List<DummyJsonProduct>? Products { get; set; }
        public int Total { get; set; }
        public int Skip { get; set; }
        public int Limit { get; set; }
    }

    private class DummyJsonProduct
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public double Price { get; set; }
        public double Rating { get; set; }
        public int Stock { get; set; }
        public string? Thumbnail { get; set; }
        public List<string>? Images { get; set; }
    }
}
