using Ardalis.Specification;
using Microsoft.eShopWeb.ApplicationCore.Entities;

namespace Microsoft.eShopWeb.ApplicationCore.Specifications;

public class CatalogFilterPaginatedSpecification : Specification<CatalogItem>
{
    /// <param name="minPrice">Inclusive lower bound on price, or null for no lower bound.</param>
    /// <param name="maxPrice">Exclusive upper bound on price, or null for no upper bound.</param>
    public CatalogFilterPaginatedSpecification(int skip, int take, int? brandId, int? typeId,
        decimal? minPrice = null, decimal? maxPrice = null)
        : base()
    {
        if (take == 0)
        {
            take = int.MaxValue;
        }
        Query
            .Where(i => (!brandId.HasValue || i.CatalogBrandId == brandId) &&
            (!typeId.HasValue || i.CatalogTypeId == typeId) &&
            (!minPrice.HasValue || i.Price >= minPrice) &&
            (!maxPrice.HasValue || i.Price < maxPrice))
            .Skip(skip).Take(take);
    }
}
