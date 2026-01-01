using System.Text.Json.Serialization;

namespace Falcon.Core.Judge;

/// <summary>
/// Response from Judge API authentication.
/// </summary>
public class JudgeAuthenticationResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
}
