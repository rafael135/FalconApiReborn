namespace Falcon.Api.Features.Users.DeleteUser;

/// <summary>
/// Result indicating successful user deletion.
/// </summary>
/// <param name="Message">A message indicating the result of the deletion.</param>
public record DeleteUserResult(string Message);
