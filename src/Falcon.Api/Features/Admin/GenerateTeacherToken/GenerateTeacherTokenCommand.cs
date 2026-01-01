using MediatR;

namespace Falcon.Api.Features.Admin.GenerateTeacherToken;

/// <summary>
/// Command to generate a teacher registration token.
/// </summary>
public record GenerateTeacherTokenCommand(
    int ExpirationHours = 168 // Default: 7 days
) : IRequest<GenerateTeacherTokenResult>;
