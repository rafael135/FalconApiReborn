using Microsoft.AspNetCore.Routing;

namespace Falcon.Api.Extensions;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
