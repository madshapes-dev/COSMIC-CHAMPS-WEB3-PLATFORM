using Amazon.GameLift;
using Amazon.GameLift.Model;
using CosmicChamps.Api.Configs;
using CosmicChamps.Api.Extensions;
using CosmicChamps.Api.Model;
using Microsoft.Extensions.Options;
using Player = Amazon.GameLift.Model.Player;

namespace CosmicChamps.Api.Services.Matchmaking;

public class FlexmatchMatchmakingService : IMatchmakingService
{
    private readonly AmazonGameLiftClient _gameLiftClient;
    private readonly GameLiftConfig _gameLiftConfig;
    private readonly StatisticsService _statisticsService;
    private readonly IPlayerRepository _playerRepository;
    private readonly IOptionsMonitor<GameData> _gameDataOption;

    public FlexmatchMatchmakingService (
        AmazonGameLiftClient gameLiftClient,
        GameLiftConfig gameLiftConfig,
        StatisticsService statisticsService,
        IPlayerRepository playerRepository,
        IOptionsMonitor<GameData> gameDataOption)
    {
        _gameLiftClient = gameLiftClient;
        _gameLiftConfig = gameLiftConfig;
        _statisticsService = statisticsService;
        _playerRepository = playerRepository;
        _gameDataOption = gameDataOption;
    }

    public async Task<string> StartMatchmaking (string playerId, string walletId, string tournamentId)
    {
        var playerAttributes = new Dictionary<string, AttributeValue>
        {
            { "walletId", new AttributeValue { S = walletId } }
        };

        var statistics = await _statisticsService.Get ();
        var player = await _playerRepository.GetAsync (playerId);
        if (player == null)
            throw new ArgumentException ($"Invalid player id {playerId}", nameof (playerId));

        playerAttributes.Add ("rating", new AttributeValue { N = player.Rating });
        playerAttributes.Add ("level", new AttributeValue { N = player.Level });

        var configurationName = _gameLiftConfig.MatchmakingConfiguration;
        var prizeBots = _gameDataOption.CurrentValue.Bots.Where (x => x.Tags.Contains (BotTag.Prize));
        if (player.GamesPlayed < 3)
        {
            configurationName = _gameLiftConfig.TutorBotMatchmakingConfiguration;
        } else if (statistics.GamesBeforePrizeBot == 0 &&
                   player.GamesBeforePrizeBot == 0 &&
                   prizeBots.Any (x => player.Rating > x.GetRatingThreshold ()))
        {
            configurationName = _gameLiftConfig.PrizeBotMatchmakingConfiguration;

            var playerPrizeBotRate = _gameDataOption.CurrentValue.PlayerPrizeBotRate;
            var playerPrizeBotRateIncrement = player.PrizeBotGamesPlayed / 5;
            player.GamesBeforePrizeBot = playerPrizeBotRate + playerPrizeBotRateIncrement + 1;
            
            await _playerRepository.UpdateAsync (player);
            await _statisticsService.ResetGamesBeforePrizeBot ();
        } else if (!string.IsNullOrEmpty (tournamentId))
        {
            configurationName = _gameLiftConfig.TournamentMatchmakingConfiguration;
            playerAttributes.Add ("tournamentId", new AttributeValue { S = tournamentId });
        }

        var startMatchMakingResponse = await _gameLiftClient.StartMatchmakingAsync (
            new Amazon.GameLift.Model.StartMatchmakingRequest
            {
                ConfigurationName = configurationName,
                Players = new List<Player>
                {
                    new()
                    {
                        PlayerId = playerId,
                        PlayerAttributes = playerAttributes
                    }
                }
            });

        return startMatchMakingResponse
            .MatchmakingTicket
            .TicketId;
    }

    public async Task<PlayerGameSession> GetPlayerGameSession (string playerId, string ticketId)
    {
        while (true)
        {
            var describeMatchMakingResponse = await _gameLiftClient.DescribeMatchmakingAsync (
                new DescribeMatchmakingRequest
                {
                    TicketIds = new List<string> { ticketId }
                });

            var matchmakingTicket = describeMatchMakingResponse.TicketList.FirstOrDefault ();
            if (matchmakingTicket == null)
                throw new ArgumentException ("Invalid ticket id", nameof (ticketId));

            var matchmakingTicketStatus = matchmakingTicket.Status;
            if (matchmakingTicketStatus == MatchmakingConfigurationStatus.COMPLETED)
            {
                var gameSessionInfo = matchmakingTicket.GameSessionConnectionInfo;
                var playerSessionInfo = gameSessionInfo.MatchedPlayerSessions.FirstOrDefault (x => x.PlayerId == playerId);
                if (playerSessionInfo == null)
                    throw new ArgumentException ("Invalid player id", nameof (playerId));

                return new PlayerGameSession
                {
                    IpAddress = gameSessionInfo.IpAddress,
                    Port = gameSessionInfo.Port,
                    DnsName = gameSessionInfo.DnsName,
                    GameSessionId = gameSessionInfo.GameSessionArn,
                    PlayerSessionId = playerSessionInfo.PlayerSessionId
                };
            }

            if (matchmakingTicketStatus == MatchmakingConfigurationStatus.TIMED_OUT)
                throw new TicketTerminatedException (TicketTerminationReason.Timeout);

            if (matchmakingTicketStatus == MatchmakingConfigurationStatus.CANCELLED)
                throw new TicketTerminatedException (TicketTerminationReason.Cancelled);

            if (matchmakingTicketStatus == MatchmakingConfigurationStatus.QUEUED ||
                matchmakingTicketStatus == MatchmakingConfigurationStatus.PLACING ||
                matchmakingTicketStatus == MatchmakingConfigurationStatus.SEARCHING)
                //
                continue;

            throw new InvalidOperationException ($"Matchmaking failed: unexpected ticket status {matchmakingTicketStatus.Value}");
        }
    }

    public async Task CancelTicket (string ticketId)
    {
        var describeMatchMakingReply = await _gameLiftClient.DescribeMatchmakingAsync (
            new DescribeMatchmakingRequest
            {
                TicketIds = new List<string> { ticketId }
            });

        var matchmakingTicket = describeMatchMakingReply.TicketList.FirstOrDefault ();
        if (matchmakingTicket == null)
            return;

        await _gameLiftClient.StopMatchmakingAsync (
            new Amazon.GameLift.Model.StopMatchmakingRequest
            {
                TicketId = ticketId
            });
    }
}