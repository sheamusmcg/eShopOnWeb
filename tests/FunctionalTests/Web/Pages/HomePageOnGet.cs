using Xunit;

namespace Microsoft.eShopWeb.FunctionalTests.Web.Pages;

[Collection("Sequential")]
public class HomePageOnGet(TestApplication factory) : IClassFixture<TestApplication>
{
    public HttpClient Client { get; } = factory.CreateClient();

    [Fact]
    public async Task ReturnsHomePageWithProductListing()
    {
        // Arrange & Act
        var response = await Client.GetAsync("/", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        var stringResponse = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Contains(".NET Bot Black Sweatshirt", stringResponse);
    }

    [Fact]
    public async Task ShowsPriceRangeFilterWithBrandAndTypeFilters()
    {
        var stringResponse = await GetPageAsync("/");

        Assert.Contains("name=\"CatalogModel.BrandFilterApplied\"", stringResponse);
        Assert.Contains("name=\"CatalogModel.TypesFilterApplied\"", stringResponse);
        Assert.Contains("name=\"CatalogModel.PriceRangeFilterApplied\"", stringResponse);
        Assert.Contains("Under $10", stringResponse);
    }

    // Seed data has 12 items: four under $10, seven from $10 to $15, and one at $15 and over.
    [Theory]
    [InlineData("?CatalogModel.PriceRangeFilterApplied=1", "Showing 4 of 4 products", "Roslyn Red Sheet", ".NET Bot Black Sweatshirt")]
    [InlineData("?CatalogModel.PriceRangeFilterApplied=3", "Showing 1 of 1 products", ".NET Bot Black Sweatshirt", "Roslyn Red Sheet")]
    [InlineData("?CatalogModel.TypesFilterApplied=3&CatalogModel.PriceRangeFilterApplied=1", "Showing 2 of 2 products", "Roslyn Red Sheet", ".NET Foundation Sheet")]
    [InlineData("?CatalogModel.PriceRangeFilterApplied=", "Showing 10 of 12 products", ".NET Bot Black Sweatshirt", "NO RESULTS")]
    public async Task FiltersCatalogByPriceRange(string query, string expectedSummary, string expectedItem, string excludedItem)
    {
        var stringResponse = await GetPageAsync("/" + query);

        Assert.Contains(expectedSummary, stringResponse);
        Assert.Contains(expectedItem, stringResponse);
        Assert.DoesNotContain(excludedItem, stringResponse);
    }

    private async Task<string> GetPageAsync(string url)
    {
        var response = await Client.GetAsync(url, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
    }
}
