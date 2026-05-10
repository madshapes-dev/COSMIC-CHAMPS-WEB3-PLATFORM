using System.Globalization;
using CosmicChamps.Api.Model;
using Microsoft.Extensions.Options;

namespace CosmicChamps.Api.Services;

public class StatisticsService
{
    private readonly IStatisticsRepository _statisticsRepository;
    private readonly IOptionsMonitor<GameData> _gameDataOption;

    public StatisticsService (IStatisticsRepository statisticsRepository, IOptionsMonitor<GameData> gameDataOption)
    {
        _statisticsRepository = statisticsRepository;
        _gameDataOption = gameDataOption;
    }

    public Task<Statistics> Get () => _statisticsRepository.Get ();

    public async Task NewGameStarted ()
    {
        var statistics = await _statisticsRepository.Get ();
        var lastGameDate = statistics.LastGameDate != null
            ? DateTime.Parse (statistics.LastGameDate, CultureInfo.InvariantCulture)
            : DateTime.Now;

        statistics.GamesPlayed++;
        statistics.GamesBeforePrizeBot = Math.Max (0, statistics.GamesBeforePrizeBot - 1);
        statistics.LastGameDate = DateTime.Now.ToString (CultureInfo.InvariantCulture);

        if (lastGameDate.DayOfWeek != DateTime.Now.DayOfWeek)
            statistics.TodayGamesPlayed = 1;
        else
            statistics.TodayGamesPlayed++;

        await _statisticsRepository.Update (statistics);
    }

    public async Task ResetGamesBeforePrizeBot ()
    {
        var statistics = await _statisticsRepository.Get ();
        statistics.GamesBeforePrizeBot = _gameDataOption.CurrentValue.PrizeBotRate + 1;

        await _statisticsRepository.Update (statistics);
    }
}