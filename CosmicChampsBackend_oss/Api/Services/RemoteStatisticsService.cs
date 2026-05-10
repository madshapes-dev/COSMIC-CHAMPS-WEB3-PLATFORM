using System.Web;
using CosmicChamps.Api.Configs;
using Microsoft.Extensions.Options;

namespace CosmicChamps.Api.Services;

public class RemoteStatisticsService
{
    private readonly RemoteStatsServiceConfig _config;
    private readonly IHttpClientFactory _httpClientFactory;

    public RemoteStatisticsService (IOptions<RemoteStatsServiceConfig> config, IHttpClientFactory httpClientFactory)
    {
        _config = config.Value;
        _httpClientFactory = httpClientFactory;
    }

    public async void ReportGameSessionStartAndForget (string gameSession, string playerIdA, string playerIB) =>
        await ReportGameSessionStart (gameSession, playerIdA, playerIB);

    public async Task ReportGameSessionStart (string gameSession, string playerIdA, string playerIdB)
    {
        using var client = _httpClientFactory.CreateClient ();
        await client.GetAsync (
            $"{_config.Endpoint}/matchstart/{HttpUtility.UrlEncode (gameSession)}/{HttpUtility.UrlEncode (playerIdA)}/{HttpUtility.UrlEncode (playerIdB)}");
    }

    public async void ReportGameSessionEndAndForget (string gameSession, string playerId) =>
        await ReportGameSessionEnd (gameSession, playerId);

    public async Task ReportGameSessionEnd (string gameSession, string playerId)
    {
        using var client = _httpClientFactory.CreateClient ();
        await client.GetAsync (
            $"{_config.Endpoint}/matchend/{HttpUtility.UrlEncode (gameSession)}/{HttpUtility.UrlEncode (string.IsNullOrEmpty (playerId) ? "NONE" : playerId)}");
    }
}