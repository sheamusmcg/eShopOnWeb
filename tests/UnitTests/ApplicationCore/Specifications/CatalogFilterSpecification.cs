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

    [Fact]
    public void MinimumPriceIsInclusive()
    {
        // Item priced exactly at the minimum (10.00) is included.
        var spec = new eShopWeb.ApplicationCore.Specifications.CatalogFilterSpecification(null, null, 10m, 15m);

        var names = spec.Evaluate(GetPricedTestItemCollection()).Select(i => i.Name).ToList();

        Assert.Contains("Priced 10.00", names);
    }

    [Fact]
    public void MaximumPriceIsExclusive()
    {
        // Items priced exactly at the maximum (15.00) are excluded.
        var spec = new eShopWeb.ApplicationCore.Specifications.CatalogFilterSpecification(null, null, 10m, 15m);

        var names = spec.Evaluate(GetPricedTestItemCollection()).Select(i => i.Name).ToList();

        Assert.DoesNotContain("Priced 15.00 Brand 1", names);
        Assert.DoesNotContain("Priced 15.00 Brand 2", names);
    }

    [Fact]
    public void FifteenDollarItemIsExcludedFromTenToFifteenAndIncludedFromFifteenToTwenty()
    {
        var tenToFifteen = new eShopWeb.ApplicationCore.Specifications.CatalogFilterSpecification(null, null, 10m, 15m);
        var fifteenToTwenty = new eShopWeb.ApplicationCore.Specifications.CatalogFilterSpecification(null, null, 15m, 20m);

        var lowerBand = tenToFifteen.Evaluate(GetPricedTestItemCollection()).Select(i => i.Name).ToList();
        var upperBand = fifteenToTwenty.Evaluate(GetPricedTestItemCollection()).Select(i => i.Name).ToList();

        Assert.DoesNotContain("Priced 15.00 Brand 1", lowerBand);
        Assert.Contains("Priced 15.00 Brand 1", upperBand);
    }

    [Theory]
    [InlineData(null, null, null, null, 8)]
    [InlineData(null, null, null, 10.0, 1)]
    [InlineData(null, null, 20.0, null, 1)]
    [InlineData(null, null, 10.0, 15.0, 3)]
    [InlineData(null, null, 15.0, 20.0, 3)]
    [InlineData(1, null, 15.0, 20.0, 2)]
    [InlineData(1, 2, 10.0, 15.0, 1)]
    [InlineData(2, null, 15.0, 20.0, 1)]
    [InlineData(2, null, 10.0, 15.0, 0)]
    [InlineData(1, 1, 20.0, null, 1)]
    public void PriceRangeComposesWithBrandAndType(int? brandId, int? typeId, double? minPrice, double? maxPrice, int expectedCount)
    {
        decimal? min = minPrice is null ? null : (decimal)minPrice;
        decimal? max = maxPrice is null ? null : (decimal)maxPrice;
        var spec = new eShopWeb.ApplicationCore.Specifications.CatalogFilterSpecification(brandId, typeId, min, max);

        var result = spec.Evaluate(GetPricedTestItemCollection()).ToList();

        Assert.Equal(expectedCount, result.Count);
        Assert.All(result, item =>
        {
            Assert.True(brandId is null || item.CatalogBrandId == brandId);
            Assert.True(typeId is null || item.CatalogTypeId == typeId);
            Assert.True(min is null || item.Price >= min);
            Assert.True(max is null || item.Price < max);
        });
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

    // Arguments are (catalogTypeId, catalogBrandId, ..., price, ...). Names encode price and brand for assertions.
    public static List<CatalogItem> GetPricedTestItemCollection()
    {
        return new List<CatalogItem>()
            {
                new CatalogItem(1, 1, "Description", "Priced 9.99", 9.99m, "FakePath"),
                new CatalogItem(1, 1, "Description", "Priced 10.00", 10.00m, "FakePath"),
                new CatalogItem(1, 1, "Description", "Priced 14.99", 14.99m, "FakePath"),
                new CatalogItem(1, 1, "Description", "Priced 15.00 Brand 1", 15.00m, "FakePath"),
                new CatalogItem(1, 2, "Description", "Priced 15.00 Brand 2", 15.00m, "FakePath"),
                new CatalogItem(2, 1, "Description", "Priced 12.00 Type 2", 12.00m, "FakePath"),
                new CatalogItem(1, 1, "Description", "Priced 19.99", 19.99m, "FakePath"),
                new CatalogItem(1, 1, "Description", "Priced 20.00", 20.00m, "FakePath"),
            };
    }
}
