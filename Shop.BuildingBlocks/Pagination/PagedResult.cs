namespace Shop.BuildingBlocks.Pagination;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();

    public int Total { get; init; }

    public int Skip { get; init; }

    public int Limit { get; init; }
}
