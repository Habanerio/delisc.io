using System.Net;
using System.Net.Http.Json;

using Common.Models;

using Links.Models;

namespace Tests.Functional.Modules;

public class LinksFunctionalTests : BaseFunctionalTests
{
    private const string API_PATH = "/links";

    [Fact]
    public async Task GetLinks_ShouldReturnOk_WhenValidRequest()
    {
        // Arrange
        var queryParams = $"?pageNo={DEFAULT_PAGE_NO}&pageSize={DEFAULT_PAGE_SIZE}";
        var requestUrl = $"{API_PATH}{queryParams}";

        // Act
        var response = await GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var results = await response.Content.ReadFromJsonAsync<PagedResults<LinkItem>>();

        Assert.IsType<PagedResults<LinkItem>>(results);

        Assert.Equal(DEFAULT_PAGE_NO, results.PageNumber);
        Assert.Equal(DEFAULT_PAGE_SIZE, results.PageSize);
        Assert.NotEmpty(results.Results);
    }

    [Theory]
    [InlineData("chipotle")]
    [InlineData("chipotle,cauliflower")]
    [InlineData("chipotle,cauliflower,food")]
    public async Task GetLinks_ShouldReturnOk_WhenValidRequestWithTags(string tags)
    {
        // Arrange
        var queryParams = $"?pageNo={DEFAULT_PAGE_NO}&pageSize={DEFAULT_PAGE_SIZE}&tags={tags}";
        var requestUrl = $"{API_PATH}{queryParams}";

        // Act
        var response = await GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var results = await response.Content.ReadFromJsonAsync<PagedResults<LinkItem>>();

        Assert.IsType<PagedResults<LinkItem>>(results);
        Assert.Equal(DEFAULT_PAGE_NO, results.PageNumber);
        Assert.Equal(DEFAULT_PAGE_SIZE, results.PageSize);
        Assert.NotEmpty(results.Results);

        var links = results.Results.ToList();

        var linkTagNames = links.SelectMany(link => link.Tags.Select(tag => tag.Name)).Distinct().ToArray();

        // Check that all `tags` that were used in the search, are present in the results
        foreach (var tag in tags.Split(',')
                     .Select(t => t.Trim())
                     .Where(t => !string.IsNullOrEmpty(t)))
        {
            Assert.Contains(tag, linkTagNames);
        }
    }

    [Fact]
    public async Task GetLinks_ShouldReturnBadRequest_WhenInvalidPageNo()
    {
        // Arrange
        var queryParams = $"?pageNo=0&pageSize={DEFAULT_PAGE_SIZE}";
        var requestUrl = $"{API_PATH}{queryParams}";

        // Act
        var response = await GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetLinks_ShouldReturnBadRequest_WhenInvalidPageSize()
    {
        // Arrange
        var queryParams = $"?pageNo={DEFAULT_PAGE_NO}&pageSize=0";
        var requestUrl = $"{API_PATH}{queryParams}";

        // Act
        var response = await GetAsync(requestUrl);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}