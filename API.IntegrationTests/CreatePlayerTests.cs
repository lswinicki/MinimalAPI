using System.Net;
using System.Net.Http.Json;
using API.Methods.Player;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace API.IntegrationTests;

public class CreatePlayerTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreatePlayer_ShouldReturnBadRequest_WhenNameIsEmpty()
    {
        var request = new CreatePlayerRequest(string.Empty);
        var response = await _client.PostAsJsonAsync("/teams/1/players", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreatePlayer_ShouldReturnOk_WhenRequestIsValid()
    {
        var request = new CreatePlayerRequest("John Doe");
        var response = await _client.PostAsJsonAsync("/teams/1/players", request);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreatePlayer_ShouldReturnBadRequest_WhenTeamDoesNotExist()
    {
        var request = new CreatePlayerRequest("John Doe");
        var response = await _client.PostAsJsonAsync("/teams/9999/players", request);
        
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);        
        problem!.Detail.Should().Be("Team not found");
    }
}