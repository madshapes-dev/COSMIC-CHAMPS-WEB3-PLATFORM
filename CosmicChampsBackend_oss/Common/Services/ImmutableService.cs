using System.Net.Http.Headers;
using System.Net.Http.Json;
using CosmicChamps.Common.Model;

namespace CosmicChamps.Common.Services;

public class ImmutableService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ImmutableService (IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ImmutableUserInfoResponse?> GetUserInfo (string token)
    {
        using var httpClient = _httpClientFactory.CreateClient ();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("Bearer", token);

        var immutableUserInfo =
            await httpClient.GetFromJsonAsync<ImmutableUserInfoResponse> (
                "https://api.sandbox.immutable.com/passport-profile/v1/user/info");

        return immutableUserInfo;
    }
}