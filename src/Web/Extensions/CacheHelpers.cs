namespace Microsoft.eShopWeb.Web.Extensions;

public static class CacheHelpers
{
    public static readonly TimeSpan DefaultCacheDuration = TimeSpan.FromSeconds(30);
    private static readonly string _itemsKeyTemplate = "items-{0}-{1}-{2}";

    // Catalog pages are keyed by page, page size and brand. The type filter
    // narrows within a brand, so the brand entry is reused for it.
    public static string GenerateCatalogItemCacheKey(int pageIndex, int itemsPage, int? brandId, int? typeId)
    {
        return string.Format(_itemsKeyTemplate, pageIndex, itemsPage, brandId);
    }

    public static string GenerateBrandsCacheKey()
    {
        return "brands";
    }

    public static string GenerateTypesCacheKey()
    {
        return "types";
    }
}
