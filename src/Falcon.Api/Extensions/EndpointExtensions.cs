using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Falcon.Api.Extensions;

/// <summary>
/// Extension methods to discover and register Minimal API endpoints that implement <see cref="IEndpoint"/>.
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    /// Scans the given <paramref name="assembly"/> for types implementing <see cref="IEndpoint"/>
    /// and registers them as scoped services.
    /// </summary>
    /// <param name="services">The service collection to register endpoints into.</param>
    /// <param name="assembly">The assembly to scan for endpoint implementations.</param>
    /// <returns>The original <see cref="IServiceCollection"/> for chaining.</returns>
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        var endpointTypes = assembly.GetTypes()
            .Where(t => typeof(IEndpoint).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in endpointTypes)
        {
            services.AddScoped(typeof(IEndpoint), type);
        }

        return services;
    }

    /// <summary>
    /// Resolves all registered <see cref="IEndpoint"/> instances and calls <see cref="IEndpoint.MapEndpoint(IEndpointRouteBuilder)"/>
    /// to add routes to the provided <paramref name="app"/> route builder.
    /// </summary>
    /// <param name="app">The route builder where endpoints will be mapped.</param>
    /// <returns>The same <see cref="IEndpointRouteBuilder"/> for chaining.</returns>
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        var scope = app.ServiceProvider.CreateScope();
        var endpoints = scope.ServiceProvider.GetServices<IEndpoint>();

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(app);
        }

        return app;
    }
}
