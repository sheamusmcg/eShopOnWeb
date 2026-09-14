using Microsoft.eShopWeb.Web;
using Microsoft.eShopWeb.Web.Extensions;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.Web.Extensions.CacheHelpersTests;

public class GenerateCatalogItemCacheKey
{
    [Fact]
    public void ReturnsCatalogItemCacheKey()
    {
        var pageIndex = 0;
        int? brandId = null;
        int? typeId = null;
        int? priceRangeId = null;

        var result = CacheHelpers.GenerateCatalogItemCacheKey(pageIndex, Constants.ITEMS_PER_PAGE, brandId, typeId, priceRangeId);

        Assert.Equal("items-0-10---", result);
    }

    [Fact]
    public void ReturnsDifferentKeysForDifferentPriceRanges()
    {
        var noPriceRange = CacheHelpers.GenerateCatalogItemCacheKey(0, Constants.ITEMS_PER_PAGE, null, null, null);
        var underTen = CacheHelpers.GenerateCatalogItemCacheKey(0, Constants.ITEMS_PER_PAGE, null, null, 1);
        var tenToFifteen = CacheHelpers.GenerateCatalogItemCacheKey(0, Constants.ITEMS_PER_PAGE, null, null, 2);

        Assert.Equal(3, new[] { noPriceRange, underTen, tenToFifteen }.Distinct().Count());
    }
}
