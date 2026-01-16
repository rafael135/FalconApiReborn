using Falcon.Core.Interfaces;
using MediatR;

namespace Falcon.Api.Features.Admin.GenerateTeacherToken;

/// <summary>
/// Handler for generating teacher registration tokens.
/// </summary>
public class GenerateTeacherTokenHandler
    : IRequestHandler<GenerateTeacherTokenCommand, GenerateTeacherTokenResult>
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<GenerateTeacherTokenHandler> _logger;

    public GenerateTeacherTokenHandler(
        ITokenService tokenService,
        ILogger<GenerateTeacherTokenHandler> logger
    )
    {
        _tokenService = tokenService;
        _logger = logger;
    }

    public Task<GenerateTeacherTokenResult> Handle(
        GenerateTeacherTokenCommand request,
        CancellationToken cancellationToken
    )
    {
        // Validate expiration hours
        if (request.ExpirationHours <= 0 || request.ExpirationHours > 720) // Max 30 days
        {
            throw new ArgumentException("Expiration hours must be between 1 and 720 (30 days)");
        }

        var expiration = TimeSpan.FromHours(request.ExpirationHours);
        var token = _tokenService.GenerateTeacherRoleInviteToken(expiration);
        var expiresAt = DateTime.UtcNow.Add(expiration);

        _logger.LogInformation(
            "Generated teacher registration token, expires at {ExpiresAt}",
            expiresAt
        );

        return Task.FromResult(new GenerateTeacherTokenResult(token, expiresAt));
    }
}
