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

        var result = CacheHelpers.GenerateCatalogItemCacheKey(pageIndex, Constants.ITEMS_PER_PAGE, brandId, typeId, null);

        Assert.Equal("items-0-10---", result);
    }

    [Fact]
    public void IncludesPriceRangeSoDifferentPriceBandsDoNotShareCacheEntries()
    {
        var withBand = CacheHelpers.GenerateCatalogItemCacheKey(0, Constants.ITEMS_PER_PAGE, null, null, "10-15");
        var withOtherBand = CacheHelpers.GenerateCatalogItemCacheKey(0, Constants.ITEMS_PER_PAGE, null, null, "15-20");

        Assert.Equal("items-0-10---10-15", withBand);
        Assert.NotEqual(withBand, withOtherBand);
    }
}
