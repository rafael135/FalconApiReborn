using System.Net.Http.Headers;

namespace Falcon.Api.IntegrationTests.Helpers;

public static class HttpClientExtensions
{
    public static void SetBearerToken(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public static void ClearAuthorization(this HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = null;
    }
}
