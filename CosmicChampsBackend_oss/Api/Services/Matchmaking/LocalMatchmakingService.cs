using System.Net;
using System.Text.Json;
using Amazon.GameLift;
using Amazon.GameLift.Model;
using CosmicChamps.Api.Configs;
using CosmicChamps.Api.Model;
using Microsoft.Extensions.Options;

namespace CosmicChamps.Api.Services.Matchmaking;

public class LocalMatchmakingService : IMatchmakingService
{
    private enum TicketStatus
    {
        WaitingPVP,
        WaitingPVE,
        Processing,
        Completed,
        Canceled,
        Timeout
    }

    private class Ticket
    {
        public readonly string Id;
        public readonly string PlayerId;
        public readonly string TournamentId;
        public readonly DateTime CreationDate;
        public TicketStatus Status;
        public PlayerGameSession? PlayerGameSession;

        public Ticket(string playerId, string tournamentId)
        {
            Id = Guid.NewGuid().ToString();
            Status = TicketStatus.WaitingPVP;
            CreationDate = DateTime.Now;
            PlayerId = playerId;
            TournamentId = tournamentId;
        }
    }

    private readonly List<Ticket> _ticketsQueue = new();
    private readonly AmazonGameLiftClient _gameLiftClient;
    private readonly ILogger<LocalMatchmakingService> _logger;
    private readonly IOptionsMonitor<LocalMatchmakingConfig> _localMatchmakingConfigOption;
    private readonly IOptionsMonitor<GameData> _gameDataOption;
    private readonly StatisticsService _statisticsService;
    private readonly IPlayerRepository _playerRepository;
    private readonly GameLiftConfig _gameLiftConfig;

    public LocalMatchmakingService(
        AmazonGameLiftClient gameLiftClient,
        ILogger<LocalMatchmakingService> logger,
        IOptionsMonitor<LocalMatchmakingConfig> localMatchmakingConfigOption,
        StatisticsService statisticsService,
        IPlayerRepository playerRepository,
        GameLiftConfig gameLiftConfig,
        IOptionsMonitor<GameData> gameDataOption)
    {
        _gameLiftClient = gameLiftClient;
        _logger = logger;
        _localMatchmakingConfigOption = localMatchmakingConfigOption;
        _statisticsService = statisticsService;
        _playerRepository = playerRepository;
        _gameLiftConfig = gameLiftConfig;
        _gameDataOption = gameDataOption;

        Task.Run(QueuePollTask);
    }

    private async Task QueuePollTask()
    {
        List<Ticket> matchTickets = new();
        List<string> playerIds = new();
        List<Ticket> waitingTickets = new();

        while (true)
        {
            matchTickets.Clear();
            playerIds.Clear();
            waitingTickets.Clear();

            lock (_ticketsQueue)
            {
                foreach (var ticket in _ticketsQueue)
                {
                    if (ticket.Status == TicketStatus.WaitingPVP)
                        waitingTickets.Add(ticket);
                }
            }

            var statistics = await _statisticsService.Get();
            var usePrizeBot = false;
            var useTutorBot = false;

            var i = 0;
            while (i < waitingTickets.Count)
            {
                var ticket = waitingTickets[i];
                var player = await _playerRepository.GetAsync(ticket.PlayerId);

                if (true || player!.GamesPlayed < 3)
                {
                    lock (ticket)
                        ticket.Status = TicketStatus.WaitingPVE;
                    useTutorBot = true;
                } else if (statistics.GamesBeforePrizeBot == 0 && player.GamesBeforePrizeBot == 0)
                {
                    lock (ticket)
                        ticket.Status = TicketStatus.WaitingPVE;
                    usePrizeBot = true;
                } else if ((DateTime.Now - ticket.CreationDate).TotalSeconds >=
                           _localMatchmakingConfigOption.CurrentValue.PVPTimeout)
                {
                    lock (ticket)
                        ticket.Status = string.IsNullOrEmpty(ticket.TournamentId)
                            ? TicketStatus.WaitingPVE
                            : TicketStatus.Timeout;
                }

                i++;
            }

            var sessionType = GameMode.None;
            var pveTickets = waitingTickets.Where(x => x.Status == TicketStatus.WaitingPVE).ToArray();
            var pvpTickets = waitingTickets.Where(x => x.Status == TicketStatus.WaitingPVP).ToArray();

            if (pveTickets.Length > 0)
                sessionType = GameMode.PVE;
            else if (pvpTickets.Length > 1 && pvpTickets[0].TournamentId == pvpTickets[1].TournamentId)
                sessionType = GameMode.PVP;

            /*_logger.LogDebug (
                "sessionType {SessionType}; pveTickets : {PveTicketsLength}; pvpTickets: {PvpTicketsLength}",
                sessionType,
                pveTickets.Length,
                pvpTickets.Length);*/

            if (sessionType == GameMode.None)
            {
                await Task.Delay(1000);
                continue;
            }

            if (usePrizeBot)
            {
                var player = await _playerRepository.GetAsync(pveTickets.First().PlayerId);

                var playerPrizeBotRate = _gameDataOption.CurrentValue.PlayerPrizeBotRate;
                var playerPrizeBotRateIncrement = player!.PrizeBotGamesPlayed / 5;
                player.GamesBeforePrizeBot = playerPrizeBotRate + playerPrizeBotRateIncrement + 1;
                
                await _playerRepository.UpdateAsync(player);
                await _statisticsService.ResetGamesBeforePrizeBot();
            }

            matchTickets.AddRange(
                sessionType switch
                {
                    GameMode.PVP => pvpTickets.Take(2),
                    GameMode.PVE => pveTickets.Take(1),
                    _ => throw new ArgumentOutOfRangeException()
                });
            playerIds.AddRange(matchTickets.Select(x => x.PlayerId));

            foreach (var matchTicket in matchTickets)
            {
                lock (matchTicket) matchTicket.Status = TicketStatus.Processing;
            }

            var fleetId = $"LocalFleet-{Guid.NewGuid().ToString()}";
            var gameSessionId = $"Local-{Guid.NewGuid().ToString()}";
            var createGameSessionReply = await _gameLiftClient.CreateGameSessionAsync(
                new CreateGameSessionRequest
                {
                    FleetId = fleetId,
                    GameSessionId = gameSessionId,
                    MaximumPlayerSessionCount = sessionType switch
                    {
                        GameMode.PVP => 2,
                        GameMode.PVE => 1,
                        _ => throw new ArgumentOutOfRangeException()
                    },
                    Name = gameSessionId,
                    GameSessionData = JsonSerializer.Serialize(
                        new LocalGameSessionData
                        {
                            PlayerIds = playerIds,
                            GameMode = sessionType,
                            TournamentId = sessionType switch
                            {
                                GameMode.PVP => pvpTickets[0].TournamentId,
                                GameMode.PVE => null,
                                _ => throw new ArgumentOutOfRangeException()
                            },
                            MatchmakingConfigurationArn = sessionType switch
                            {
                                GameMode.PVE when usePrizeBot => _gameLiftConfig.PrizeBotMatchmakingConfiguration,
                                GameMode.PVE when useTutorBot => _gameLiftConfig.TutorBotMatchmakingConfiguration,
                                GameMode.PVP when !string.IsNullOrEmpty(pvpTickets[0].TournamentId) =>
                                    _gameLiftConfig.TournamentMatchmakingConfiguration,
                                _ => "CosmicChamps-Local"
                            }
                        })
                });

            do
            {
                var describeGameSessionsReply = await _gameLiftClient.DescribeGameSessionsAsync(
                    new DescribeGameSessionsRequest
                    {
                        GameSessionId = gameSessionId
                    });

                var gameSessions = describeGameSessionsReply.GameSessions;
                if (!gameSessions.Any())
                    throw new InvalidOperationException("Just created game sessions not found");

                if (gameSessions.First().Status == GameSessionStatus.ACTIVE)
                    break;
            } while (true);

            var gameSession = createGameSessionReply.GameSession;
            var createPlayerSessionsReply = await _gameLiftClient.CreatePlayerSessionsAsync(
                new CreatePlayerSessionsRequest
                {
                    GameSessionId = gameSession.GameSessionId,
                    PlayerIds = playerIds
                });

            string GetLocalIPAddress()
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    var ipStr = ip.ToString();
                    if (ipStr.StartsWith("192.168.1"))
                        return ipStr;
                }

                throw new Exception("No network adapters with an IPv4 address in the system!");
            }

            var playerSessions = createPlayerSessionsReply.PlayerSessions;
            foreach (var matchTicket in matchTickets)
            {
                lock (matchTicket)
                {
                    matchTicket.PlayerGameSession = new PlayerGameSession
                    {
                        IpAddress = GetLocalIPAddress(),
                        Port = gameSession.Port,
                        DnsName = gameSession.DnsName,
                        GameSessionId = gameSession.GameSessionId,
                        PlayerSessionId = playerSessions.Find(x => x.PlayerId == matchTicket.PlayerId)?.PlayerSessionId,
                    };
                    matchTicket.Status = TicketStatus.Completed;
                }
            }

            await Task.Delay(1000);
        }
    }

    public Task<string> StartMatchmaking(string playerId, string walletId, string tournamentId)
    {
        var ticket = new Ticket(playerId, tournamentId);

        lock (_ticketsQueue) _ticketsQueue.Add(ticket);

        return Task.FromResult(ticket.Id);
    }

    public async Task<PlayerGameSession> GetPlayerGameSession(string playerId, string ticketId)
    {
        Ticket? ticket;
        lock (_ticketsQueue) ticket = _ticketsQueue.Find(x => x.Id == ticketId);

        if (ticket == null)
            throw new ArgumentException("Invalid ticket id", nameof(ticketId));

        while (true)
        {
            await Task.Delay(1000);

            TicketStatus status;
            lock (ticket) status = ticket.Status;

            if (status is TicketStatus.WaitingPVP or TicketStatus.WaitingPVE or TicketStatus.Processing)
                continue;

            switch (status)
            {
                case TicketStatus.Completed when ticket.PlayerGameSession == null:
                    throw new InvalidOperationException("Null player game session");
                case TicketStatus.Completed:
                    return ticket.PlayerGameSession;
                case TicketStatus.Timeout:
                    throw new TicketTerminatedException(TicketTerminationReason.Timeout);
                case TicketStatus.Canceled:
                    throw new TicketTerminatedException(TicketTerminationReason.Cancelled);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public Task CancelTicket(string ticketId)
    {
        lock (_ticketsQueue)
        {
            var ticketIndex = _ticketsQueue.FindIndex(x => x.Id == ticketId);
            if (ticketIndex < 0 || _ticketsQueue[ticketIndex].Status != TicketStatus.WaitingPVP)
                return Task.CompletedTask;

            lock (_ticketsQueue[ticketIndex]) _ticketsQueue[ticketIndex].Status = TicketStatus.Canceled;
        }

        return Task.CompletedTask;
    }
}