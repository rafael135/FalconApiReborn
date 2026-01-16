namespace Falcon.Api.Features.Admin.GenerateTeacherToken;

/// <summary>
/// Result containing generated teacher token.
/// </summary>
public record GenerateTeacherTokenResult(string Token, DateTime ExpiresAt);
