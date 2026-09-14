namespace Microsoft.eShopWeb.Web.ViewModels;

/// <summary>
/// A preset price range shown in the catalog price filter. MinPrice is inclusive and MaxPrice is exclusive,
/// so a boundary price such as 10.00 falls in exactly one range.
/// </summary>
public record CatalogPriceRange(int Id, string Label, decimal? MinPrice, decimal? MaxPrice)
{
    public static readonly IReadOnlyList<CatalogPriceRange> Options = new List<CatalogPriceRange>
    {
        new(1, "Under $10", null, 10m),
        new(2, "$10 to $15", 10m, 15m),
        new(3, "$15 and over", 15m, null),
    };

    public static CatalogPriceRange? FindById(int? id) => Options.FirstOrDefault(r => r.Id == id);
}
