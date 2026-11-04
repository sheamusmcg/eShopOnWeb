using Ardalis.Specification;
using Microsoft.eShopWeb.ApplicationCore.Entities;

namespace Microsoft.eShopWeb.ApplicationCore.Specifications;

public class CatalogFilterSpecification : Specification<CatalogItem>
{
    /// <param name="minPrice">Inclusive lower bound on price, or null for no lower bound.</param>
    /// <param name="maxPrice">Exclusive upper bound on price, or null for no upper bound.</param>
    public CatalogFilterSpecification(int? brandId, int? typeId, decimal? minPrice = null, decimal? maxPrice = null)
    {
        Query.Where(i => (!brandId.HasValue || i.CatalogBrandId == brandId) &&
            (!typeId.HasValue || i.CatalogTypeId == typeId) &&
            (!minPrice.HasValue || i.Price >= minPrice) &&
            (!maxPrice.HasValue || i.Price < maxPrice));
    }
}
