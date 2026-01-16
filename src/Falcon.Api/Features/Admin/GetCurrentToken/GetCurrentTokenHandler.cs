using Falcon.Core.Interfaces;
using MediatR;

namespace Falcon.Api.Features.Admin.GetCurrentToken;

/// <summary>
/// Handler for getting current teacher registration token.
/// </summary>
public class GetCurrentTokenHandler : IRequestHandler<GetCurrentTokenQuery, GetCurrentTokenResult>
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<GetCurrentTokenHandler> _logger;

    public GetCurrentTokenHandler(
        ITokenService tokenService,
        ILogger<GetCurrentTokenHandler> logger
    )
    {
        _tokenService = tokenService;
        _logger = logger;
    }

    public Task<GetCurrentTokenResult> Handle(
        GetCurrentTokenQuery request,
        CancellationToken cancellationToken
    )
    {
        var token = _tokenService.FetchPrivateAccessToken();

        if (token != null)
        {
            _logger.LogInformation("Current teacher token retrieved from cache");
        }
        else
        {
            _logger.LogInformation("No active teacher token found");
        }

        return Task.FromResult(new GetCurrentTokenResult(token));
    }
}
