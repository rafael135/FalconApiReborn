using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Extensions;

public static class ControllerBaseExtensions
{
    /// <summary>
    /// Sets the authentication cookie in the HTTP response.
    /// </summary>
    /// <param name="controller">The controller instance to extend.</param>
    /// <param name="token">The authentication token to set in the cookie.</param>
    public static void SetAuthCookie(this ControllerBase controller, string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.UtcNow.AddDays(1),
            SameSite = SameSiteMode.Lax,
            Path = "/",
        };
        controller.Response.Cookies.Append("CompetitionAuthToken", token, cookieOptions);
    }

    /// <summary>
    /// Deletes the authentication cookie from the HTTP response.
    /// </summary>
    /// <param name="controller">The controller instance to extend.</param>
    public static void DeleteAuthCookie(this ControllerBase controller)
    {
        CookieOptions cookieOptions = new CookieOptions()
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.UtcNow.AddDays(-1),
            SameSite = SameSiteMode.Lax,
            Path = "/",
        };

        controller.Response.Cookies.Append("CompetitionAuthToken", "", cookieOptions);
    }
}
