using Microsoft.eShopWeb.ApplicationCore.Entities;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Specifications;

public class CatalogFilterPaginatedSpecification
{
    [Fact]
    public void ReturnsAllCatalogItems()
    {
        var spec = new eShopWeb.ApplicationCore.Specifications.CatalogFilterPaginatedSpecification(0, 10, null, null);

        var result = spec.Evaluate(GetTestCollection());

        Assert.NotNull(result);
        Assert.Equal(4, result.ToList().Count);
    }

    [Fact]
    public void Returns2CatalogItemsWithSameBrandAndTypeId()
    {
        var spec = new eShopWeb.ApplicationCore.Specifications.CatalogFilterPaginatedSpecification(0, 10, 1, 1);

        var result = spec.Evaluate(GetTestCollection()).ToList();

        Assert.NotNull(result);
        Assert.Equal(2, result.ToList().Count);
    }

    [Fact]
    public void PriceRangeIncludesMinimumAndExcludesMaximum()
    {
        var spec = new eShopWeb.ApplicationCore.Specifications.CatalogFilterPaginatedSpecification(0, 10, null, null, 10m, 15m);

        var names = spec.Evaluate(GetPricedTestCollection()).Select(i => i.Name).ToList();

        Assert.Contains("Priced 10.00", names);
        Assert.DoesNotContain("Priced 15.00 Brand 1", names);
        Assert.DoesNotContain("Priced 15.00 Brand 2", names);
    }

    [Fact]
    public void FifteenDollarItemIsIncludedFromFifteenToTwentyOnly()
    {
        var fifteenToTwenty = new eShopWeb.ApplicationCore.Specifications.CatalogFilterPaginatedSpecification(0, 10, null, null, 15m, 20m);
        var tenToFifteen = new eShopWeb.ApplicationCore.Specifications.CatalogFilterPaginatedSpecification(0, 10, null, null, 10m, 15m);

        Assert.Contains("Priced 15.00 Brand 1", fifteenToTwenty.Evaluate(GetPricedTestCollection()).Select(i => i.Name));
        Assert.DoesNotContain("Priced 15.00 Brand 1", tenToFifteen.Evaluate(GetPricedTestCollection()).Select(i => i.Name));
    }

    [Fact]
    public void PriceRangeWithBrandReturnsOnlyItemsMatchingBoth()
    {
        var spec = new eShopWeb.ApplicationCore.Specifications.CatalogFilterPaginatedSpecification(0, 10, 1, null, 15m, 20m);

        var names = spec.Evaluate(GetPricedTestCollection()).Select(i => i.Name).ToList();

        Assert.Equal(new[] { "Priced 15.00 Brand 1", "Priced 19.99" }, names);
    }

    [Theory]
    [InlineData(null, null, null, null)]
    [InlineData(null, null, 10.0, 15.0)]
    [InlineData(null, null, 15.0, 20.0)]
    [InlineData(1, null, 15.0, 20.0)]
    [InlineData(1, 2, 10.0, 15.0)]
    [InlineData(2, null, 10.0, 15.0)]
    [InlineData(1, 1, 20.0, null)]
    public void TotalCountMatchesFilterSpecificationForSameCriteria(int? brandId, int? typeId, double? minPrice, double? maxPrice)
    {
        decimal? min = minPrice is null ? null : (decimal)minPrice;
        decimal? max = maxPrice is null ? null : (decimal)maxPrice;
        // AC7: the list and the count apply the same criteria, so the unpaged list always equals the total.
        var items = GetPricedTestCollection();
        var countSpec = new eShopWeb.ApplicationCore.Specifications.CatalogFilterSpecification(brandId, typeId, min, max);
        var listSpec = new eShopWeb.ApplicationCore.Specifications.CatalogFilterPaginatedSpecification(0, 0, brandId, typeId, min, max);

        var total = countSpec.Evaluate(items).Count();
        var listed = listSpec.Evaluate(items).ToList();

        Assert.Equal(total, listed.Count);
    }

    [Fact]
    public void PagingAppliesAfterPriceFilter()
    {
        var firstPage = new eShopWeb.ApplicationCore.Specifications.CatalogFilterPaginatedSpecification(0, 2, null, null, 10m, 20m);
        var secondPage = new eShopWeb.ApplicationCore.Specifications.CatalogFilterPaginatedSpecification(2, 2, null, null, 10m, 20m);

        var first = firstPage.Evaluate(GetPricedTestCollection()).Select(i => i.Name).ToList();
        var second = secondPage.Evaluate(GetPricedTestCollection()).Select(i => i.Name).ToList();

        Assert.Equal(new[] { "Priced 10.00", "Priced 14.99" }, first);
        Assert.Equal(new[] { "Priced 15.00 Brand 1", "Priced 15.00 Brand 2" }, second);
    }

    private List<CatalogItem> GetTestCollection()
    {
        var catalogItemList = new List<CatalogItem>();

        catalogItemList.Add(new CatalogItem(1, 1, "Item 1", "Item 1", 1.00m, "TestUri1"));
        catalogItemList.Add(new CatalogItem(1, 1, "Item 1.5", "Item 1.5", 1.50m, "TestUri1"));
        catalogItemList.Add(new CatalogItem(2, 2, "Item 2", "Item 2", 2.00m, "TestUri2"));
        catalogItemList.Add(new CatalogItem(3, 3, "Item 3", "Item 3", 3.00m, "TestUri3"));

        return catalogItemList;
    }

    // Arguments are (catalogTypeId, catalogBrandId, description, name, price, pictureUri).
    private static List<CatalogItem> GetPricedTestCollection()
    {
        return new List<CatalogItem>()
        {
            new CatalogItem(1, 1, "Description", "Priced 9.99", 9.99m, "TestUri"),
            new CatalogItem(1, 1, "Description", "Priced 10.00", 10.00m, "TestUri"),
            new CatalogItem(1, 1, "Description", "Priced 14.99", 14.99m, "TestUri"),
            new CatalogItem(1, 1, "Description", "Priced 15.00 Brand 1", 15.00m, "TestUri"),
            new CatalogItem(1, 2, "Description", "Priced 15.00 Brand 2", 15.00m, "TestUri"),
            new CatalogItem(2, 1, "Description", "Priced 12.00 Type 2", 12.00m, "TestUri"),
            new CatalogItem(1, 1, "Description", "Priced 19.99", 19.99m, "TestUri"),
            new CatalogItem(1, 1, "Description", "Priced 20.00", 20.00m, "TestUri"),
        };
    }
}
