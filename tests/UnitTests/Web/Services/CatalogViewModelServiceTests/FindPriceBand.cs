using Microsoft.eShopWeb.Web.Services;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.Web.Services.CatalogViewModelServiceTests;

public class FindPriceBand
{
    [Theory]
    [InlineData("under-10", null, 10.0)]
    [InlineData("10-15", 10.0, 15.0)]
    [InlineData("15-20", 15.0, 20.0)]
    [InlineData("20-and-over", 20.0, null)]
    public void MapsEachPresetBandToItsMinimumAndMaximum(string key, double? expectedMin, double? expectedMax)
    {
        var band = CatalogViewModelService.FindPriceBand(key);

        Assert.NotNull(band);
        Assert.Equal(key, band.Key);
        Assert.Equal(expectedMin is null ? null : (decimal)expectedMin, band.MinPrice);
        Assert.Equal(expectedMax is null ? null : (decimal)expectedMax, band.MaxPrice);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("10-20")]
    [InlineData("UNDER-10")]
    public void UnrecognizedValueMeansNoPriceFilter(string? key)
    {
        Assert.Null(CatalogViewModelService.FindPriceBand(key));
    }
}
