using MediatR;

namespace Falcon.Api.Features.Admin.GetCurrentToken;

/// <summary>
/// Query to get the current teacher registration token.
/// </summary>
public record GetCurrentTokenQuery() : IRequest<GetCurrentTokenResult>;
