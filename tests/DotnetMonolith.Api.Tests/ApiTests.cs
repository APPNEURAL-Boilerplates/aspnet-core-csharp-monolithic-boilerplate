using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace DotnetMonolith.Api.Tests;

[TestClass]
public sealed class ApiTests
{
    private CustomWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    [TestInitialize]
    public void SetUp()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [TestCleanup]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [TestMethod]
    public async Task Root_ReturnsRunningStatus()
    {
        using var response = await _client.GetAsync("/");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        using var json = await ReadJsonAsync(response);
        Assert.IsTrue(json.RootElement.GetProperty("ok").GetBoolean());
        Assert.AreEqual("running", json.RootElement.GetProperty("data").GetProperty("status").GetString());
    }

    [TestMethod]
    public async Task Health_ReturnsHealthyStatus()
    {
        using var response = await _client.GetAsync("/health");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        using var json = await ReadJsonAsync(response);
        Assert.IsTrue(json.RootElement.GetProperty("ok").GetBoolean());
        Assert.AreEqual("healthy", json.RootElement.GetProperty("data").GetProperty("status").GetString());
    }

    [TestMethod]
    public async Task Users_CanCreateUser()
    {
        var payload = new
        {
            name = "Grace Hopper",
            email = $"grace-{Guid.NewGuid():N}@example.com"
        };

        using var response = await _client.PostAsJsonAsync("/api/v1/users", payload);
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

        using var json = await ReadJsonAsync(response);
        Assert.IsTrue(json.RootElement.GetProperty("ok").GetBoolean());
        Assert.AreEqual("Grace Hopper", json.RootElement.GetProperty("data").GetProperty("name").GetString());
    }

    [TestMethod]
    public async Task Users_InvalidJson_ReturnsBadRequest()
    {
        using var content = new StringContent("{ invalid json", Encoding.UTF8, "application/json");
        using var response = await _client.PostAsync("/api/v1/users", content);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

        using var json = await ReadJsonAsync(response);
        Assert.IsFalse(json.RootElement.GetProperty("ok").GetBoolean());
        Assert.AreEqual("VALIDATION_ERROR", json.RootElement.GetProperty("error").GetProperty("code").GetString());
    }

    [TestMethod]
    public async Task UnknownRoute_ReturnsNotFound()
    {
        using var response = await _client.GetAsync("/does-not-exist");

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);

        using var json = await ReadJsonAsync(response);
        Assert.IsFalse(json.RootElement.GetProperty("ok").GetBoolean());
        Assert.AreEqual("NOT_FOUND", json.RootElement.GetProperty("error").GetProperty("code").GetString());
    }

    [TestMethod]
    public async Task UnsupportedMethod_ReturnsMethodNotAllowed()
    {
        using var response = await _client.PostAsync("/health", content: null);

        Assert.AreEqual(HttpStatusCode.MethodNotAllowed, response.StatusCode);

        using var json = await ReadJsonAsync(response);
        Assert.IsFalse(json.RootElement.GetProperty("ok").GetBoolean());
        Assert.AreEqual("METHOD_NOT_ALLOWED", json.RootElement.GetProperty("error").GetProperty("code").GetString());
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(body);
    }
}
