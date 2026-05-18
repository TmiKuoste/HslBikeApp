using System.Net;
using System.Text;
using HslBikeApp.Services;

namespace HslBikeApp.Tests.Services;

public class OpenDataServiceTests
{
    [Fact]
    public async Task FetchOpenDataAsync_WhenResponseIsSuccessful_ReturnsDeserialisedTimeSeries()
    {
        var responseJson =
            """
            [
              {
                "sourceId": "uimastadion",
                "displayName": "Uimastadion",
                "lat": 60.1857,
                "lon": 24.9282,
                "attributionUrl": "https://example.com/uimastadion",
                "unit": "visitors",
                "description": "Live visitor count",
                "timestamps": ["2026-05-18T10:00:00+00:00", "2026-05-18T10:15:00+00:00"],
                "values": [185, 192]
              }
            ]
            """;

        var httpClient = new HttpClient(new StubHttpMessageHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        })));
        var service = new OpenDataService(httpClient, "https://aggregator.example");

        var series = await service.FetchOpenDataAsync();

        var single = Assert.Single(series);
        Assert.Equal("uimastadion", single.SourceId);
        Assert.Equal("Uimastadion", single.DisplayName);
        Assert.Equal(60.1857, single.Lat);
        Assert.Equal(24.9282, single.Lon);
        Assert.Equal("https://example.com/uimastadion", single.AttributionUrl);
        Assert.Equal("visitors", single.Unit);
        Assert.Equal("Live visitor count", single.Description);
        Assert.Equal(2, single.Timestamps.Count);
        Assert.Equal(new[] { 185.0, 192.0 }, single.Values);
        Assert.True(service.LastFetchSucceeded);
        Assert.Null(service.LastErrorMessage);
    }

    [Fact]
    public async Task FetchOpenDataAsync_WhenUnitAndDescriptionAreOmitted_LeavesThemNull()
    {
        var responseJson =
            """
            [
              {
                "sourceId": "legacy-source",
                "displayName": "Legacy",
                "lat": 60.0,
                "lon": 25.0,
                "attributionUrl": "https://example.com/legacy",
                "timestamps": [],
                "values": []
              }
            ]
            """;

        var httpClient = new HttpClient(new StubHttpMessageHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        })));
        var service = new OpenDataService(httpClient, "https://aggregator.example");

        var single = Assert.Single(await service.FetchOpenDataAsync());

        Assert.Null(single.Unit);
        Assert.Null(single.Description);
    }

    [Fact]
    public async Task FetchOpenDataAsync_WhenBaseUrlHasTrailingSlash_UsesOpenDataEndpoint()
    {
        HttpRequestMessage? capturedRequest = null;
        var httpClient = new HttpClient(new StubHttpMessageHandler((request, _) =>
        {
            capturedRequest = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]", Encoding.UTF8, "application/json")
            });
        }));
        var service = new OpenDataService(httpClient, "https://aggregator.example/");

        await service.FetchOpenDataAsync();

        Assert.NotNull(capturedRequest);
        Assert.Equal("https://aggregator.example/api/open-data", capturedRequest.RequestUri?.ToString());
    }

    [Fact]
    public async Task FetchOpenDataAsync_WhenHttpRequestFails_ReturnsEmptyListAndRecordsError()
    {
        var httpClient = new HttpClient(new StubHttpMessageHandler((_, _) => throw new HttpRequestException("boom")));
        var service = new OpenDataService(httpClient, "https://aggregator.example");

        var series = await service.FetchOpenDataAsync();

        Assert.Empty(series);
        Assert.False(service.LastFetchSucceeded);
        Assert.Equal("Could not load open data from the aggregator backend. GET https://aggregator.example/api/open-data failed.", service.LastErrorMessage);
    }

    [Fact]
    public async Task FetchOpenDataAsync_WhenEndpointReturnsNotFound_SetsConfigurationErrorMessage()
    {
        var httpClient = new HttpClient(new StubHttpMessageHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound))));
        var service = new OpenDataService(httpClient, "https://aggregator.example");

        var series = await service.FetchOpenDataAsync();

        Assert.Empty(series);
        Assert.False(service.LastFetchSucceeded);
        Assert.Equal("Could not load open data from the aggregator backend. GET https://aggregator.example/api/open-data returned 404 Not Found. Check AggregatorBaseUrl in wwwroot/appsettings.json.", service.LastErrorMessage);
    }

    [Fact]
    public void OpenDataService_WhenBaseUrlIsNull_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new OpenDataService(new HttpClient(), null!));
    }

    [Fact]
    public void OpenDataService_WhenHttpClientIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new OpenDataService(null!, "https://example.com"));
    }
}
