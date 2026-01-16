using System.Text.Json.Serialization;

namespace Falcon.Infrastructure.Judge.Models;

/// <summary>
/// Response from Judge API authentication.
/// </summary>
public class JudgeAuthenticationResponse
{
    /// <summary>
    /// The access token returned by Judge API authentication endpoint.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
}
