using Microsoft.eShopWeb.ApplicationCore.Entities;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.ApplicationCore.Specifications;

public class CatalogFilterSpecification
{
    [Theory]
    [InlineData(null, null, 5)]
    [InlineData(1, null, 3)]
    [InlineData(2, null, 2)]
    [InlineData(null, 1, 2)]
    [InlineData(null, 3, 1)]
    [InlineData(1, 3, 1)]
    [InlineData(2, 3, 0)]
    public void MatchesExpectedNumberOfItems(int? brandId, int? typeId, int expectedCount)
    {
        var spec = new eShopWeb.ApplicationCore.Specifications.CatalogFilterSpecification(brandId, typeId);

        var result = spec.Evaluate(GetTestItemCollection()).ToList();

        Assert.Equal(expectedCount, result.Count());
    }

    public static TheoryData<int?, decimal?, decimal?, int> PriceRangeCases => new()
    {
        { null, null, null, 5 },
        { null, null, 10m, 1 },
        { null, 10m, 15m, 2 },
        { null, 15m, null, 2 },
        { 1, 10m, 15m, 2 },
        { 2, 10m, 15m, 0 },
        { 2, 15m, null, 2 },
    };

    [Theory]
    [MemberData(nameof(PriceRangeCases))]
    public void MatchesExpectedNumberOfItemsInPriceRange(int? brandId, decimal? minPrice, decimal? maxPrice, int expectedCount)
    {
        var spec = new eShopWeb.ApplicationCore.Specifications.CatalogFilterSpecification(brandId, null, minPrice, maxPrice);

        var result = spec.Evaluate(GetPricedTestItemCollection()).ToList();

        Assert.Equal(expectedCount, result.Count);
    }

    public List<CatalogItem> GetTestItemCollection()
    {
        return new List<CatalogItem>()
            {
                new CatalogItem(1, 1, "Description", "Name", 0, "FakePath"),
                new CatalogItem(2, 1, "Description", "Name", 0, "FakePath"),
                new CatalogItem(3, 1, "Description", "Name", 0, "FakePath"),
                new CatalogItem(1, 2, "Description", "Name", 0, "FakePath"),
                new CatalogItem(2, 2, "Description", "Name", 0, "FakePath"),
            };
    }

    // Prices sit on and around the 10.00 and 15.00 range boundaries: min is inclusive, max is exclusive.
    public List<CatalogItem> GetPricedTestItemCollection()
    {
        return new List<CatalogItem>()
            {
                new CatalogItem(1, 1, "Description", "Name", 8.50m, "FakePath"),
                new CatalogItem(2, 1, "Description", "Name", 10.00m, "FakePath"),
                new CatalogItem(3, 1, "Description", "Name", 12.00m, "FakePath"),
                new CatalogItem(1, 2, "Description", "Name", 15.00m, "FakePath"),
                new CatalogItem(2, 2, "Description", "Name", 19.50m, "FakePath"),
            };
    }
}
