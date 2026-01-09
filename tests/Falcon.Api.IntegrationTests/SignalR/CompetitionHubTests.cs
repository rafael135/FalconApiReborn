using Falcon.Api.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR.Client;

namespace Falcon.Api.IntegrationTests.SignalR;

public class CompetitionHubTests : TestBase
{
    public CompetitionHubTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Connect_When_Authenticated()
    {
        // Arrange
        var (_, token) = await CreateStudentAsync();
        var hubUrl = $"{Factory.Server.BaseAddress}hubs/competition";

        var connection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.HttpMessageHandlerFactory = _ => Factory.Server.CreateHandler();
                options.AccessTokenProvider = () => Task.FromResult<string?>(token);
            })
            .Build();

        // Act & Assert
        await connection.StartAsync();
        connection.State.Should().Be(HubConnectionState.Connected);
        await connection.StopAsync();
    }

    [Fact]
    public async Task Should_Disconnect_When_NotAuthenticated()
    {
        // Arrange
        var hubUrl = $"{Factory.Server.BaseAddress}hubs/competition";

        var connection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.HttpMessageHandlerFactory = _ => Factory.Server.CreateHandler();
            })
            .Build();

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(async () => await connection.StartAsync());
    }
}
