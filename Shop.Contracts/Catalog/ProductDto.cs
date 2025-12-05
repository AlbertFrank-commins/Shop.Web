namespace Shop.Contracts.Catalog;

public class ProductDto
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public string Brand { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public double Rating { get; init; }

    public int Stock { get; init; }

    public string Thumbnail { get; init; } = string.Empty;

    public IReadOnlyList<string> Images { get; init; } = Array.Empty<string>();
}
