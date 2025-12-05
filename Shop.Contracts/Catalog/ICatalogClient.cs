using Shop.BuildingBlocks.Pagination;

namespace Shop.Contracts.Catalog;

public interface ICatalogClient
{
    /// <summary>
    /// Obtiene una lista paginada de productos.
    /// </summary>
    /// <param name="skip">Cantidad de elementos a saltar (offset).</param>
    /// <param name="limit">Cantidad máxima de elementos a devolver.</param>
    /// <param name="search">Texto de búsqueda (nombre, descripción...).</param>
    /// <param name="category">Filtrar por categoría (opcional).</param>
    Task<PagedResult<ProductDto>> GetProductsAsync(
        int skip = 0,
        int limit = 30,
        string? search = null,
        string? category = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un producto por su Id.
    /// </summary>
    Task<ProductDto?> GetProductByIdAsync(
        int id,
        CancellationToken cancellationToken = default);
}
