using FluentAssertions;
using System.Net;

namespace Falcon.Api.IntegrationTests.Features.Competitions;

public class GetCompetitionsTests : TestBase
{
    public GetCompetitionsTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_GettingCompetitions()
    {
        // Arrange
        await CreateTemplateAsync();

        // Act
        var response = await HttpClient.GetAsync("/api/Competition");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
