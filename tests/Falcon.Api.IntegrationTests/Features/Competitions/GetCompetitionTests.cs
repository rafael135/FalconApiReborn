using FluentAssertions;
using System.Net;

namespace Falcon.Api.IntegrationTests.Features.Competitions;

public class GetCompetitionTests : TestBase
{
    public GetCompetitionTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_CompetitionExists()
    {
        // Arrange
        var competition = await CreateTemplateAsync();

        // Act
        var response = await HttpClient.GetAsync($"/api/Competition/{competition.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Should_ReturnNotFound_When_CompetitionDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await HttpClient.GetAsync($"/api/Competition/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
