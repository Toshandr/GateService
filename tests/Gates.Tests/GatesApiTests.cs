using System.Net;
using System.Net.Http.Json;
using System.Text;
using Gates.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Gates.Tests;

public class GatesApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private const string Url = "/api/gates/validate";
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task ValidCode_Returns200_AndIsValidTrue()
    {
        var response = await _client.PostAsJsonAsync(Url, new { accessCode = "Dragon7!" });
        var body = await response.Content.ReadFromJsonAsync<ValidateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.True(body.IsValid);
        Assert.Empty(body.Errors);
        Assert.Equal("Код принят. Ворота открываются.", body.Message);
    }

    [Fact]
    public async Task InvalidCode_Returns200_WithAllErrors()
    {
        var response = await _client.PostAsJsonAsync(Url, new { accessCode = "dragon123" });
        var body = await response.Content.ReadFromJsonAsync<ValidateResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.False(body.IsValid);
        Assert.Equal(3, body.Errors.Count);
        Assert.Equal("Код отклонён.", body.Message);
    }

    [Theory]
    [InlineData("""{"accessCode": ""}""")]
    [InlineData("""{"accessCode": "   "}""")]
    [InlineData("""{"accessCode": null}""")]
    [InlineData("""{}""")]
    public async Task MissingOrEmptyCode_Returns400(string json)
    {
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(Url, content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task EmptyBody_Returns400()
    {
        var content = new StringContent("", Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(Url, content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
