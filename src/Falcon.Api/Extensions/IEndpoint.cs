using Microsoft.AspNetCore.Routing;

namespace Falcon.Api.Extensions;

/// <summary>
/// Minimal abstraction for a discoverable endpoint that knows how to map itself into an <see cref="IEndpointRouteBuilder"/>.
/// Implement this interface for feature-specific endpoint registration using the project's endpoint discovery mechanism.
/// </summary>
public interface IEndpoint
{
    /// <summary>
    /// Maps routes and handlers to the provided <paramref name="app"/> route builder.
    /// </summary>
    /// <param name="app">The <see cref="IEndpointRouteBuilder"/> to add routes to.</param>
    void MapEndpoint(IEndpointRouteBuilder app);
}
