using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.GameLift;
using Amazon.GameLift.Model;
using CosmicChamps.Api.Configs;
using CosmicChamps.Api.Extensions;
using CosmicChamps.Api.Model;
using CosmicChamps.Api.Model.Validation;
using CosmicChamps.Api.Protos;
using CosmicChamps.Api.Services.Matchmaking;
using CosmicChamps.Common.Configs;
using CosmicChamps.Common.Model;
using CosmicChamps.Common.Services;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Player = CosmicChamps.Api.Model.Player;

namespace CosmicChamps.Api.Services;

[Authorize]
public class GameService : Game.GameBase
{
    private const string DefaultLevel = "Level";
    private const string Level2 = "Level2";
    private const string DefaultBot = "default";
    private const string TutorBot = "tutor";

    private readonly ILogger<GameService> _logger;
    private readonly AmazonGameLiftClient _gameLiftClient;
    private readonly ValidationService _validationService;
    private readonly IGameSessionRepository _gameSessionRepository;
    private readonly IMatchmakingService _matchmakingService;
    private readonly IErrorReportRepository _errorReportRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IMatchReportRepository _matchReportRepository;
    private readonly RemoteStatisticsService _remoteStatisticsService;
    private readonly IOptionsMonitor<GameData> _gameDataOption;
    private readonly IOptionsMonitor<List<HUDSkin>> _hudSkinsDataOption;
    private readonly IOptionsMonitor<List<string>> _restrictedNicknamesOptions;
    private readonly INewsRepository _newsRepository;
    private readonly StatisticsService _statisticsService;
    private readonly IOptionsMonitor<GuestAccountsConfig> _guestAccountsConfigOption;
    private readonly IGuestCredentialsRepository _guestCredentialsRepository;
    private readonly IImmutableCredentialsRepository _immutableCredentialsRepository;
    private readonly IAmazonCognitoIdentityProvider _identityProvider;
    private readonly AWSCognitoConfig _cognitoConfig;
    private readonly GameLiftConfig _gameLiftConfig;
    private readonly Random _random = new();
    private readonly ImmutableService _immutableService;

    public GameService(
        ILogger<GameService> logger,
        AmazonGameLiftClient gameLiftClient,
        ValidationService validationService,
        IGameSessionRepository gameSessionRepository,
        IErrorReportRepository errorReportRepository,
        IMatchmakingService matchmakingService,
        IPlayerRepository playerRepository,
        RemoteStatisticsService remoteStatisticsService,
        IOptionsMonitor<GameData> gameDataOption,
        INewsRepository newsRepository,
        IOptionsMonitor<List<HUDSkin>> hudSkinsDataOption,
        ITournamentRepository tournamentRepository,
        IOptionsMonitor<List<string>> restrictedNicknamesOptions,
        StatisticsService statisticsService,
        IMatchReportRepository matchReportRepository,
        IOptionsMonitor<GuestAccountsConfig> guestAccountsConfigOption,
        IGuestCredentialsRepository guestCredentialsRepository,
        IAmazonCognitoIdentityProvider identityProvider,
        AWSCognitoConfig cognitoConfig,
        GameLiftConfig gameLiftConfig,
        IImmutableCredentialsRepository immutableCredentialsRepository,
        ImmutableService immutableService)
    {
        _logger = logger;
        _gameLiftClient = gameLiftClient;
        _gameDataOption = gameDataOption;
        _newsRepository = newsRepository;
        _hudSkinsDataOption = hudSkinsDataOption;
        _tournamentRepository = tournamentRepository;
        _restrictedNicknamesOptions = restrictedNicknamesOptions;
        _statisticsService = statisticsService;
        _matchReportRepository = matchReportRepository;
        _guestAccountsConfigOption = guestAccountsConfigOption;
        _guestCredentialsRepository = guestCredentialsRepository;
        _identityProvider = identityProvider;
        _cognitoConfig = cognitoConfig;
        _validationService = validationService;
        _gameSessionRepository = gameSessionRepository;
        _errorReportRepository = errorReportRepository;
        _matchmakingService = matchmakingService;
        _playerRepository = playerRepository;
        _remoteStatisticsService = remoteStatisticsService;
        _gameLiftConfig = gameLiftConfig;
        _immutableCredentialsRepository = immutableCredentialsRepository;
        _immutableService = immutableService;
    }

    private async Task ValidatePlayerInventory(Player player)
    {
        var isBot = _gameDataOption
            .CurrentValue
            .Bots
            .GetIds()
            .Contains(player.Id);
        if (isBot)
            return;

        if (string.IsNullOrEmpty(player.LinkedWalletId))
        {
            player.ResetToNoWallet(_gameDataOption.CurrentValue);
        } else
        {
            var inventory = await _validationService.GetInventory(player.LinkedWalletId);
            inventory.ApplyToPlayer(player, _gameDataOption.CurrentValue, _hudSkinsDataOption.CurrentValue);
            player.ValidateDecks(_gameDataOption.CurrentValue);
        }

        await _playerRepository.UpdateAsync(player);
    }

    private void ValidateNickname(string nickname)
    {
        if (!Regex.IsMatch(nickname, "^[0-9A-Za-z]{3,12}$"))
            throw new RpcException(
                new Status(
                    StatusCode.InvalidArgument,
                    "Nickname should contains only letters and digits and be 3-12 symbols long"));
    }

    private void ValidateNicknameRestrictions(string nickname)
    {
        var nicknameLowerCase = nickname.ToLowerInvariant();
        foreach (var restricted in _restrictedNicknamesOptions.CurrentValue)
        {
            if (nicknameLowerCase.Contains(restricted))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Nickname is restricted"));
        }
    }

    public override Task<GameDataReply> GetGameData(GameDataRequest request, ServerCallContext context)
    {
        return Task.FromResult(
            new GameDataReply
            {
                GameData = _gameDataOption.CurrentValue
            });
    }

    private async Task<Model.PlayerGameSession?> TryJoinExistingGameSession(string playerId, string tournamentId)
    {
        var gameSession = await _gameSessionRepository.GetByPlayerId(playerId, tournamentId);
        if (gameSession == null)
            return null;

        try
        {
            var createPlayerSessionReply = await _gameLiftClient.CreatePlayerSessionAsync(
                new CreatePlayerSessionRequest
                {
                    GameSessionId = gameSession.Id,
                    PlayerId = playerId
                });

            return new Model.PlayerGameSession
            {
                GameSession = gameSession,
                PlayerSessionId = createPlayerSessionReply.PlayerSession.PlayerSessionId
            };
        } catch (InvalidGameSessionStatusException)
        {
            await _gameSessionRepository.DeleteAsync(gameSession.Id);
            return null;
        }
    }

    public override async Task StartMatchmaking(
        StartMatchmakingRequest request,
        IServerStreamWriter<StartMatchmakingReply> responseStream,
        ServerCallContext context)
    {
        var playerId = context.GetPlayerId();
        var player = await _playerRepository.GetAsync(playerId);
        if (player == null)
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "Player data doesn't exist"));

        if (!await _validationService.PerformKillswitchCheck(player.WalletId))
            throw new RpcException(new Status(StatusCode.PermissionDenied, "You aren't validated to access the battle"));

        var tournamentId = request.TournamentId;
        if (!string.IsNullOrEmpty(tournamentId))
        {
            var tournament = await _tournamentRepository.GetAsync(tournamentId);
            if (tournament == null)
                throw new RpcException(new Status(StatusCode.FailedPrecondition, "Invalid tournament id"));

            if (!tournament.Players.Contains(playerId))
                throw new RpcException(
                    new Status(
                        StatusCode.PermissionDenied,
                        "Player is not permitted to participate the tournament"));
        }

        var existingPlayerSession = await TryJoinExistingGameSession(playerId, tournamentId);
        if (existingPlayerSession != null)
        {
            await responseStream.WriteAsync(
                new StartMatchmakingReply
                {
                    GameSession = new PlayerGameSession
                    {
                        IpAddress = existingPlayerSession.GameSession.IpAddress,
                        Port = existingPlayerSession.GameSession.Port,
                        WebGLPort = existingPlayerSession.GameSession.Port + 1,
                        DnsName = existingPlayerSession.GameSession.DnsName,
                        GameSessionId = existingPlayerSession.GameSession.Id,
                        PlayerSessionId = existingPlayerSession.PlayerSessionId
                    }
                });

            return;
        }

        if (player.Decks == null)
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "Player has no decks"));

        var deck = player.Decks[player.ActiveDeckIndex];
        if (deck.Cards.Any(x => x == null))
            throw new RpcException(new Status(StatusCode.PermissionDenied, "Fill in the deck completely"));

        try
        {
            var ticketId = await _matchmakingService.StartMatchmaking(playerId, player.WalletId, tournamentId);
            await responseStream.WriteAsync(
                new StartMatchmakingReply
                {
                    TicketId = ticketId
                });

            var playerGameSession = await _matchmakingService.GetPlayerGameSession(playerId, ticketId);
            await responseStream.WriteAsync(
                new StartMatchmakingReply
                {
                    GameSession = new PlayerGameSession
                    {
                        IpAddress = playerGameSession.IpAddress,
                        Port = playerGameSession.Port,
                        WebGLPort = playerGameSession.Port + 1,
                        DnsName = playerGameSession.DnsName,
                        GameSessionId = playerGameSession.GameSessionId,
                        PlayerSessionId = playerGameSession.PlayerSessionId
                    }
                });
        } catch (TicketTerminatedException ticketTerminatedException)
        {
            await responseStream.WriteAsync(
                ticketTerminatedException.Reason switch
                {
                    TicketTerminationReason.Cancelled => new StartMatchmakingReply { Canceled = true },
                    TicketTerminationReason.Timeout => new StartMatchmakingReply { Timeout = true },
                    _ => throw new ArgumentOutOfRangeException()
                });
        } catch (Exception e)
        {
            _logger.LogError(e, default);
            throw new RpcException(new Status(StatusCode.Unknown, e.Message));
        }
    }

    public override async Task<StopMatchmakingReply> StopMatchmaking(StopMatchmakingRequest request, ServerCallContext context)
    {
        await _matchmakingService.CancelTicket(request.TicketId);
        return new StopMatchmakingReply();
    }

    public override async Task<StartGameSessionReply> StartGameSession(
        StartGameSessionRequest request,
        ServerCallContext context)
    {
        List<Player> players;
        string level;
        if (request.GameSession.PlayerIds.Count == 1)
        {
            var player = await _playerRepository.GetAsync(request.GameSession.PlayerIds[0]);
            if (player == null)
                throw new RpcException(new Status(StatusCode.FailedPrecondition, "Player data doesn't exist"));

            players = new List<Player> { player };

            var usePrizeBot = request.GameSession.MatchmakerConfigurationArn.Contains(
                _gameLiftConfig.PrizeBotMatchmakingConfiguration,
                StringComparison.InvariantCultureIgnoreCase);

            var useTutorBot = request.GameSession.MatchmakerConfigurationArn.Contains(
                _gameLiftConfig.TutorBotMatchmakingConfiguration,
                StringComparison.InvariantCultureIgnoreCase);

            var gameData = _gameDataOption.CurrentValue;
            var gameDataBots = gameData.Bots;
            BotLevel? botLevel = null;

            if (useTutorBot)
            {
                botLevel = gameDataBots.First(x => x.Id == TutorBot).Levels[0];
            } else if (
                usePrizeBot &&
                gameDataBots
                    .Where(x => x.Tags.Contains(BotTag.Prize))
                    .ToList()
                    .GetWeightedRandom(x => x.MatchmakingWeight, out var prizeBot))
            {
                botLevel = prizeBot
                    .Levels
                    .TakeWhile(x => player.Level > x.LevelThreshold || player.Rating > x.RatingThreshold)
                    .LastOrDefault();
            }

            botLevel ??= gameDataBots.First(x => x.Id == DefaultBot)
                .Levels
                .TakeWhile(x => player.Level > x.LevelThreshold || player.Rating > x.RatingThreshold)
                .Last();

            if (botLevel == null)
                throw new RpcException(new Status(StatusCode.FailedPrecondition, "Unable to match the bot"));

            players.Add(botLevel.ToPlayer(gameData));
            level = usePrizeBot
                ? Level2
                : GetRegularModeLevel();
        } else
        {
            players = (await _playerRepository.GetAsync(request.GameSession.PlayerIds.ToArray())).ToList();
            if (players.Count != 2)
                throw new RpcException(new Status(StatusCode.FailedPrecondition, "Unable to get players data"));

            level = GetRegularModeLevel();
        }

        await _gameSessionRepository.CreateAsync(
            new Model.GameSession
            {
                Id = request.GameSession.Id,
                IpAddress = request.GameSession.IpAddress,
                Port = request.GameSession.Port,
                DnsName = request.GameSession.DnsName,
                PlayerIds = players.Select(x => x.Id).ToHashSet(),
                TournamentId = request.GameSession.TournamentId,
                StartDate = DateTimeOffset.Now.ToString()
            });

        foreach (var player in players.Where(x => x.AccountType != Model.AccountType.Bot))
        {
            player.GamesBeforePrizeBot = Math.Max(0, player.GamesBeforePrizeBot - 1);
        }

        await ValidatePlayerInventory(players.First());
        await ValidatePlayerInventory(players.Last());

        // _statisticsService.ReportGameSessionStartAndForget (request.GameSession.Id, players.First ().Id, players.Last ().Id);
        _remoteStatisticsService.ReportGameSessionStartAndForget(
            request.GameSession.Id,
            players.First().WalletId,
            players.Last().WalletId);

        await _statisticsService.NewGameStarted();

        return new StartGameSessionReply
        {
            Players = { players.Select(x => x.ToProto()) },
            Level = level
        };

        string GetRegularModeLevel()
        {
            var level2Chance = _random.Next(0, 100);
            return level2Chance <= _gameDataOption.CurrentValue.Level2Chance ? Level2 : DefaultLevel;
        }
    }

    public override async Task<StopGameSessionReply> StopGameSession(StopGameSessionRequest request, ServerCallContext context)
    {
        var gameSession = await _gameSessionRepository.GetAsync(request.GameSessionId);
        var players = await _playerRepository.GetAsync(gameSession.PlayerIds);
        var gameData = _gameDataOption.CurrentValue;
        var botIds = gameData.Bots.GetIds();
        var botId = gameSession.PlayerIds.FirstOrDefault(x => botIds.Contains(x));
        if (!string.IsNullOrEmpty(botId))
        {
            var botLevel = gameData.Bots.GetBotLevel(botId);
            if (botLevel == null)
                throw new RpcException(new Status(StatusCode.FailedPrecondition, "Unable to find bot data"));

            players.Add(botLevel.ToPlayer(gameData));
        }

        //_logger.Log (LogLevel.Debug, "AdjustPlayerRating players {Ids}", string.Join (", ", players.Select (x => x.Id)));

        void AdjustPlayerRating(Player player, Player opponent, int playerRating, int opponentRating)
        {
            var isBot = botIds.Contains(player.Id);
            if (isBot)
                return;

            var now = DateTimeOffset.Now;
            player.GamesPlayed++;
            player.LastPlayedGameDate = now.ToString();
            
            var opponentBot = gameData.Bots.FirstOrDefault(x => x.Levels.Any(y => y.Id == opponent.Id));
            if (opponentBot != null && opponentBot.Tags.Contains(BotTag.Prize))
            {
                var prizeBotGamesPlayedTimestamp = string.IsNullOrEmpty(player.PrizeBotGamesPlayedTimestamp) ? now : DateTimeOffset.Parse(player.PrizeBotGamesPlayedTimestamp);
                if (now.DayOfYear != prizeBotGamesPlayedTimestamp.DayOfYear)
                    player.PrizeBotGamesPlayed = 0;

                player.PrizeBotGamesPlayedTimestamp = now.ToString();
                player.PrizeBotGamesPlayed++;
            }

            if (request.Drawn)
                return;

            var k = 15f;
            if (player is { GamesPlayed: < 5, Rating: < 1300 })
            {
                k = 25f;
            } else
                k = player.Rating switch
                {
                    > 1400 => 7f,
                    < 1200 => 20f,
                    _ => k
                };

            // var k = player.Rating > 1400 ? 7f : 15f;
            var isWinner = player.Id == request.WinnerId;
            var e = 1f / (1f + MathF.Pow(10, (opponentRating - playerRating) / 300f));
            var s = isWinner ? 1f : 0f;
            var oldRating = player.Rating;
            // var playerRatingStash = player.Rating;

            player.Rating =
                !isWinner &&
                opponent.Id == _gameDataOption.CurrentValue.Bots.First(x => x.Id == TutorBot).Levels[0].Id
                    ? oldRating
                    : (int)Math.Round(oldRating + k * (s - e));
        }

        void GrantMissionShards(Player player)
        {
            var isBot = botIds.Contains(player.Id);
            if (isBot)
                return;
            
            player.MissionGamesPlayed++;
            foreach (var mission in gameData.Missions.List)
            {
                if (mission.GamesCount != player.MissionGamesPlayed)
                    continue;

                if (mission.Reward.Kind == _gameDataOption.CurrentValue.UniversalShardsId)
                    player.UniversalShards += mission.Reward.Amount;
                else
                    player.GrantShards(mission.Reward.Kind, mission.Reward.Amount);
                
                player.BattleRewards = (player.BattleRewards ?? Array.Empty<Model.PlayerBattleReward>())
                    .Append(
                        new Model.PlayerBattleReward
                        {
                            Shards = new []{new Model.PlayerCardShards
                                {
                                    Id = mission.Reward.Kind,
                                    Amount = mission.Reward.Amount
                                }
                            },
                            OldRating = player.Rating,
                            NewRating = player.Rating,
                            Result = Model.BattleResult.Mission
                        })
                    .ToArray();
                
                break;
            }
        }

        void GrantShards(Player player, Player opponent, int ratingInc)
        {
            var isBot = botIds.Contains(player.Id);
            if (isBot)
                return;

            var isWinner = !request.Drawn && player.Id == request.WinnerId;
            var isOpponentBot = botIds.Contains(opponent.Id);
            var deck = player.Decks![player.ActiveDeckIndex];
            var shards = new Dictionary<string, int>();

            int cardsToGrant;
            (int, int) grantRange;
            if (request.Drawn)
            {
                cardsToGrant = player.Rating > opponent.Rating ? 2 : 3;
                grantRange = (2, 4);
            } else
            {
                cardsToGrant = isWinner ? 4 : 2;
                grantRange = isWinner ? (3, 6) : (2, 4);
            }

            for (var i = 0; i < cardsToGrant; i++)
            {
                var cardIndex = _random.Next(0, deck.Cards.Length);
                var cardData = _gameDataOption.CurrentValue.Cards.First(x => x.Id == deck.Cards[cardIndex]!.Id);
                var shardsAmount = shards.GetValueOrDefault(cardData.UpgradeShardId, 0);
                shardsAmount += _random.Next(grantRange.Item1, grantRange.Item2 + 1);
                shards[cardData.UpgradeShardId] = shardsAmount;
            }

            var universalShardsReward = 0;
            if (isWinner)
            {
                universalShardsReward = (int)Math.Ceiling(ratingInc * 0.5);

                if (isOpponentBot)
                {
                    foreach (var shard in shards)
                    {
                        shards[shard.Key] = Math.Max(1, shard.Value - 1);
                    }

                    universalShardsReward = Math.Max(1, universalShardsReward - 1);
                }
            }

            player.UniversalShards += universalShardsReward;
            foreach (var shard in shards)
            {
                var playerCardShards = player.CardShards.FirstOrDefault(x => x.Id == shard.Key);
                if (playerCardShards == null)
                {
                    playerCardShards = new Model.PlayerCardShards
                    {
                        Id = shard.Key,
                        Amount = 0
                    };

                    player.CardShards = player.CardShards.Append(playerCardShards).ToArray();
                }

                playerCardShards.Amount += shard.Value;
            }

            if (universalShardsReward > 0)
                shards.Add(gameData.UniversalShardsId, universalShardsReward);

            player.BattleRewards = (player.BattleRewards ?? Array.Empty<Model.PlayerBattleReward>())
                .Append(
                    new Model.PlayerBattleReward
                    {
                        Shards = shards
                            .Select(x => new Model.PlayerCardShards
                            {
                                Id = x.Key,
                                Amount = x.Value
                            })
                            .ToArray(),
                        OldRating = player.Rating - ratingInc,
                        NewRating = player.Rating,
                        Result = request.Drawn
                            ? Model.BattleResult.Drawn
                            : isWinner
                                ? Model.BattleResult.Victory
                                : Model.BattleResult.Defeat
                    })
                .ToArray();
        }

        var player = players.First();
        var playerRatingStash = player.Rating;
        var opponent = players.Last();
        var opponentRatingStash = opponent.Rating;

        MatchReport.Player ConvertPlayer(Player sourcePlayer) =>
            new()
            {
                Id = sourcePlayer.Id,
                WalletId = sourcePlayer.WalletId,
                LinkedWalletId = sourcePlayer.LinkedWalletId,
                Nickname = sourcePlayer.Nickname,
                Deck = sourcePlayer.Decks == null
                    ? Array.Empty<MatchReport.Player.Card>()
                    : sourcePlayer.Decks[sourcePlayer.ActiveDeckIndex]
                        .Cards
                        .Where(x => x != null)
                        .Select(x => new MatchReport.Player.Card
                        {
                            Id = x!.Id,
                            Skin = x.UnitSkin
                        })
                        .ToArray(),
                Boosts = sourcePlayer.Boosts == null
                    ? Array.Empty<MatchReport.Player.Boost>()
                    : sourcePlayer
                        .Boosts
                        .Select(x => new MatchReport.Player.Boost
                        {
                            Id = x.Id,
                            Hp = x.Hp,
                            Damage = x.Damage,
                            DeathDamage = x.DeathDamage
                        })
                        .ToArray(),
                HUDSkin = sourcePlayer.HUDSkin,
                Rating = sourcePlayer.Rating,
                PrizeBotGamesPlayed = sourcePlayer.PrizeBotGamesPlayed
            };

        var playerA = ConvertPlayer(player);
        var playerB = ConvertPlayer(opponent);
        var matchReport = new MatchReport
        {
            Id = Guid.NewGuid().ToString(),
            PlayerA = playerA.Id,
            PlayerB = playerB.Id,
            WinnerId = request.Drawn
                ? "DRAWN"
                : request.WinnerId,
            Players = new[] { playerA, playerB },
            Date = DateTime.Parse(gameSession.StartDate!).ToString("yyyy/MM/dd HH:mm:ss zzz"),
            Duration = (int)(string.IsNullOrEmpty(gameSession.StartDate)
                ? 0
                : DateTimeOffset.Now.Subtract(DateTimeOffset.Parse(gameSession.StartDate)).TotalSeconds),
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
        };

        await _matchReportRepository.CreateAsync(matchReport);

        AdjustPlayerRating(player, opponent, playerRatingStash, opponentRatingStash);
        AdjustPlayerRating(opponent, player, opponentRatingStash, playerRatingStash);

        GrantShards(player, opponent, player.Rating - playerRatingStash);
        GrantShards(opponent, player, opponent.Rating - opponentRatingStash);
        
        if (gameData.Missions.Enabled)
        {
            GrantMissionShards(player);
            GrantMissionShards(opponent);
        }
        
        await _gameSessionRepository.DeleteAsync(request.GameSessionId);
        if (player.AccountType != Model.AccountType.Bot)
            await _playerRepository.UpdateAsync(player);

        if (opponent.AccountType != Model.AccountType.Bot)
            await _playerRepository.UpdateAsync(opponent);

        /*_statisticsService.ReportGameSessionEndAndForget (
            request.GameSessionId,
            request.WinnerId);*/
        _remoteStatisticsService.ReportGameSessionEndAndForget(
            request.GameSessionId,
            player.Id == request.WinnerId ? player.WalletId : opponent.WalletId);

        return new StopGameSessionReply();
    }

    private async Task<string> SuggestPlayerNickname(string email)
    {
        var nickname = Regex.Replace(email.Split('@')[0], "[^0-9A-Za-z]+", string.Empty);
        if (nickname.Length > 12)
            nickname = nickname.Substring(0, 12);

        if (!await _playerRepository.IsNicknameUnique(nickname))
        {
            var prefixedNicknames = await _playerRepository.GetPrefixedNicknames(nickname);
            var regex = $"^{nickname}([\\d]+)?$";
            var postfixes = prefixedNicknames
                .Where(x => Regex.IsMatch(x, regex))
                .Select(x => Regex.Matches(x, regex).First().Groups[1].Value)
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(int.Parse)
                .ToArray();

            var postfix = postfixes.Length > 0 ? postfixes.Max() + 1 : 1;
            nickname = $"{nickname}{postfix}";
        }

        return nickname;
    }

    public override async Task<PlayerDataReply> GetPlayerData(PlayerDataRequest request, ServerCallContext context)
    {
        var user = context
            .GetHttpContext()
            .User;
        var playerId = user.GetPlayerId();
        var player = await _playerRepository.GetAsync(playerId);

        if (player == null)
        {
            var gameData = _gameDataOption.CurrentValue;
            var email = user.Claims.First(x => x.Type == ClaimTypes.Email).Value;
            var nickname = await SuggestPlayerNickname(email);
            var walletId = await _validationService.GetWalletId(playerId);
            var immutableCredentials = await _immutableCredentialsRepository.GetByPlayerIdAsync(playerId);

            player = new Player
            {
                Id = playerId,
                WalletId = walletId,
                ImmutableWalletId = immutableCredentials?.WalletId,
                ImmutableId = immutableCredentials?.ImmutableId,
                Nickname = nickname,
                Email = email,
                Units = gameData.Units.Where(x => !x.Disabled)
                    .Select(x => new Model.PlayerUnit
                    {
                        Id = x.Id,
                        Skins = new[] { x.Id }
                    })
                    .ToArray(),
                Cards = gameData.Cards
                    .Where(x => !(x.Disabled || gameData.Units.First(y => y.Id == x.UnitId).Disabled))
                    .Select(x => new Model.PlayerCard
                    {
                        Id = x.Id,
                        Level = 0
                    })
                    .ToArray(),
                Rating = 1200,
                ShipCards = new[]
                {
                    new Model.PlayerShipCard
                    {
                        Id = "standard_ship",
                        Skins = new[] { "default" }
                    }
                },
                ShipSlot = new Model.PlayerShipSlot
                {
                    Id = "standard_ship",
                    Skin = "default"
                },
                NicknameChangeCount = 1,
                SignUpCompleted = false,
                Emojis = new[] { "ram", "coy", "inv" },
                AccountType = email.EndsWith(_guestAccountsConfigOption.CurrentValue.EmailDomain)
                    ? Model.AccountType.Guest
                    : immutableCredentials != null
                        ? Model.AccountType.Immutable
                        : Model.AccountType.Standard,
                UniversalShards = 0,
                CardShards = gameData.Cards.Select(x => x.UpgradeShardId)
                    .Distinct()
                    .Select(x => new Model.PlayerCardShards
                    {
                        Id = x,
                        Amount = 0
                    })
                    .ToArray(),
                Level = 0,
                Exp = 0,
                GamesBeforePrizeBot = gameData.PlayerPrizeBotRate
            };

            /*_logger.Log (LogLevel.Information, "player {Id} cards {Cards}", player.Id, player.Cards.Select (x => x.Id));*/

            await _playerRepository.CreateAsync(player);
        }

        if (player.Decks == null || player.Decks.Length == 0)
        {
            player.Decks = _gameDataOption
                .CurrentValue
                .DeckPresets
                .Select(x => new Model.PlayerDeck
                {
                    PresetId = x.Id,
                    Cards = x
                        .Cards
                        .Select(y => y == null ? null : new Model.PlayerDeckCard { Id = y.Id, UnitSkin = y.Skin })
                        .ToArray()
                })
                .ToArray();
            player.ActiveDeckIndex = 0;
        }

        await ValidatePlayerInventory(player);
        return new PlayerDataReply { PlayerData = player.ToProto() };
    }

    public override async Task<ReportErrorReply> ReportError(ReportErrorRequest request, ServerCallContext context)
    {
        var errorReport = new ErrorReport
        {
            Id = Guid.NewGuid().ToString(),
            Version = request.Version,
            Platform = request.Platform,
            Date = request.Date,
            GameSession = request.GameSession,
            Message = request.Message,
            Stacktrace = request.Stacktrace,
            Device = new Model.Device
            {
                Id = request.Device.Id,
                Model = request.Device.Model,
                Graphics = new Model.DeviceGraphics
                {
                    Id = request.Device.Graphics.Id,
                    Name = request.Device.Graphics.Name,
                    Type = request.Device.Graphics.Type,
                    Vendor = request.Device.Graphics.Vendor,
                    VendorID = request.Device.Graphics.VendorID,
                    Version = request.Device.Graphics.Version,
                    MemorySize = request.Device.Graphics.MemorySize
                },
                OperatingSystem = new Model.DeviceOperatingSystem
                {
                    Name = request.Device.OperatingSystem.Name,
                    Family = request.Device.OperatingSystem.Family
                }
            }
        };

        await _errorReportRepository.CreateAsync(errorReport);
        // await Task.Delay (TimeSpan.FromSeconds (5f));

        return new ReportErrorReply
        {
            ReportId = errorReport.Id
        };
    }

    public override async Task<UpdateDecksReply> UpdateDecks(UpdateDecksRequest request, ServerCallContext context)
    {
        var playerId = context.GetPlayerId();
        var player = await _playerRepository.GetAsync(playerId);
        if (player == null)
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "No player data found"));

        if (player.Decks == null)
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "No player decks found"));

        foreach (var deckUpdate in request.DeckUpdates)
        {
            if (deckUpdate.DeckIndex < 0 || deckUpdate.DeckIndex >= player.Decks.Length)
                throw new RpcException(new Status(StatusCode.FailedPrecondition, "Invalid deck index"));

            var deck = player.Decks[deckUpdate.DeckIndex];
            deck.PresetId = deckUpdate.PresetId;
            deck.Cards = deckUpdate
                .Cards
                .Select(x => x?.ToModel())
                .ToArray();

            player.ValidateDeck(deckUpdate.DeckIndex, _gameDataOption.CurrentValue);
        }

        player.ActiveDeckIndex = Math.Clamp(request.ActiveDeckIndex, 0, 2);

        await _playerRepository.UpdateAsync(player);

        return new UpdateDecksReply();
    }

    public override async Task<NewsReply> GetNews(NewsRequest request, ServerCallContext context)
    {
        var news = await _newsRepository.GetLatest();
        var reply = new NewsReply();

        do
        {
            if (news == null)
                break;

            var playerId = context.GetPlayerId();
            var player = await _playerRepository.GetAsync(playerId);
            if (player == null)
                throw new RpcException(new Status(StatusCode.FailedPrecondition, "No player data found"));

            if (news.Id == player.LatestReadNews)
                break;

            player.LatestReadNews = news.Id;
            await _playerRepository.UpdateAsync(player);

            reply.News = news.ToProto();
        } while (false);

        return reply;
    }

    public override async Task<CompleteSignUpReply> CompleteSignUp(CompleteSignUpRequest request, ServerCallContext context)
    {
        ValidateNicknameRestrictions(request.Nickname);

        var playerId = context.GetPlayerId();
        var player = await _playerRepository.GetAsync(playerId);
        if (player == null)
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "No player data found"));

        if (player.Nickname != request.Nickname && !await _playerRepository.IsNicknameUnique(request.Nickname))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Nickname already taken"));

        ValidateNickname(request.Nickname);

        player.Nickname = request.Nickname;
        player.SignUpCompleted = true;

        await _playerRepository.UpdateAsync(player);
        return new CompleteSignUpReply();
    }

    public override async Task<SaveProfileReply> SaveProfile(SaveProfileRequest request, ServerCallContext context)
    {
        var playerId = context.GetPlayerId();
        var player = await _playerRepository.GetAsync(playerId);
        if (player == null)
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "No player data found"));

        var somethingChanged = false;

        if (request.NicknameCase == SaveProfileRequest.NicknameOneofCase.NicknameValue)
        {
            ValidateNickname(request.NicknameValue);
            ValidateNicknameRestrictions(request.NicknameValue);

            if (!await _playerRepository.IsNicknameUnique(request.NicknameValue))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Nickname already taken"));

            if (player.NicknameChangeCount == 0)
                throw new RpcException(new Status(StatusCode.FailedPrecondition, "No more nickname changes"));

            player.Nickname = request.NicknameValue;
            player.NicknameChangeCount--;
            somethingChanged = true;
        }

        if (somethingChanged)
            await _playerRepository.UpdateAsync(player);

        return new SaveProfileReply
        {
            NicknameChangeCount = player.NicknameChangeCount
        };
    }

    public override async Task<UnbindWalletReply> UnbindAlgorandWallet(
        UnbindWalletRequest request,
        ServerCallContext context)
    {
        var playerId = context.GetPlayerId();
        var player = await _playerRepository.GetAsync(playerId);
        if (player == null)
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "No player data found"));

        player.LinkedWalletId = null;
        await ValidatePlayerInventory(player);

        return new UnbindWalletReply
        {
            Player = player.ToProto()
        };
    }

    public override async Task<UnbindWalletReply> UnbindImmutableWallet(UnbindWalletRequest request, ServerCallContext context)
    {
        var playerId = context.GetPlayerId();
        var player = await _playerRepository.GetAsync(playerId);
        if (player == null)
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "No player data found"));

        if (player.AccountType == Model.AccountType.Immutable)
            throw new RpcException(
                new Status(StatusCode.FailedPrecondition, "Unable to unbind immutable wallet from immutable account"));

        player.ImmutableWalletId = null;
        await ValidatePlayerInventory(player);

        return new UnbindWalletReply
        {
            Player = player.ToProto()
        };
    }

    public override async Task<BindEmailToGuestReply> BindEmailToGuest(
        BindEmailToGuestRequest request,
        ServerCallContext context)
    {
        var playerId = context.GetPlayerId();
        var deviceId = request.DeviceId;
        var email = request.Email;

        if (email.EndsWith($"@{_guestAccountsConfigOption.CurrentValue.EmailDomain}"))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Forbidden email domain"));

        var player = await _playerRepository.GetAsync(playerId);
        if (player == null)
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "No player data found"));

        var guestCredentials = await _guestCredentialsRepository.GetAsync(deviceId);
        if (guestCredentials == null)
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "No device data found"));

        if (guestCredentials.PlayerId != playerId)
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "Player doesn't match device"));

        try
        {
            do
            {
                try
                {
                    var getUserResponse = await _identityProvider.AdminGetUserAsync(
                        new AdminGetUserRequest
                        {
                            Username = email,
                            UserPoolId = _cognitoConfig.UserPoolId
                        });

                    if (getUserResponse.HttpStatusCode == HttpStatusCode.OK)
                        throw new Exception("Email already taken");
                } catch (UserNotFoundException)
                {
                }

                /*var setPasswordResponse = await _identityProvider.AdminSetUserPasswordAsync (
                    new AdminSetUserPasswordRequest
                    {
                        Password = password,
                        Permanent = true,
                        Username = guestCredentials.Email,
                        UserPoolId = _cognitoConfig.UserPoolId
                    });

                if (setPasswordResponse.HttpStatusCode != HttpStatusCode.OK)
                    break;*/

                var updateAttributesResponse = await _identityProvider.AdminUpdateUserAttributesAsync(
                    new AdminUpdateUserAttributesRequest
                    {
                        UserAttributes = new List<AttributeType>
                        {
                            new() { Name = "email", Value = email }
                        },
                        Username = guestCredentials.Email,
                        UserPoolId = _cognitoConfig.UserPoolId
                    });

                if (updateAttributesResponse.HttpStatusCode != HttpStatusCode.OK)
                    break;

                /*player.IsGuest = false;
                player.Email = email;
                player.EmailVerified = false;

                await _playerRepository.UpdateAsync (player);
                await _guestCredentialsRepository.DeleteAsync (deviceId);*/

                return new BindEmailToGuestReply();
            } while (false);
        } catch (Exception e)
        {
            _logger.LogError("Failed to bind email: {Exception}", e.Message);
            throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
        }

        throw new RpcException(new Status(StatusCode.Unknown, "Failed to bind email"));
    }

    public override async Task<ConfirmBindEmailToGuestReply> ConfirmBindEmailToGuest(
        ConfirmBindEmailToGuestRequest request,
        ServerCallContext context)
    {
        var playerId = context.GetPlayerId();
        var deviceId = request.DeviceId;
        var email = request.Email;
        var password = request.Password;
        var code = request.Code;

        if (email.EndsWith($"@{_guestAccountsConfigOption.CurrentValue.EmailDomain}"))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Forbidden email domain"));

        var player = await _playerRepository.GetAsync(playerId);
        if (player == null)
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "No player data found"));

        var guestCredentials = await _guestCredentialsRepository.GetAsync(deviceId);
        if (guestCredentials == null)
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "No device data found"));

        if (guestCredentials.PlayerId != playerId)
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "Player doesn't match device"));

        try
        {
            do
            {
                var verifyEmailResponse = await _identityProvider.VerifyUserAttributeAsync(
                    new VerifyUserAttributeRequest
                    {
                        AccessToken = request.Token,
                        AttributeName = "email",
                        Code = code
                    });

                if (verifyEmailResponse.HttpStatusCode != HttpStatusCode.OK)
                    break;

                player.AccountType = Model.AccountType.Standard;
                player.Email = email;

                await _playerRepository.UpdateAsync(player);
                await _guestCredentialsRepository.DeleteAsync(deviceId);

                var setPasswordResponse = await _identityProvider.AdminSetUserPasswordAsync(
                    new AdminSetUserPasswordRequest
                    {
                        Password = password,
                        Permanent = true,
                        Username = email,
                        UserPoolId = _cognitoConfig.UserPoolId
                    });

                if (setPasswordResponse.HttpStatusCode != HttpStatusCode.OK)
                    break;

                return new ConfirmBindEmailToGuestReply
                {
                    PlayerData = player.ToProto()
                };
            } while (false);
        } catch (Exception e)
        {
            _logger.LogError("Failed to confirm bind email: {Exception}", e.Message);
            throw new RpcException(
                new Status(
                    StatusCode.InvalidArgument,
                    e
                        .Message
                        .Replace("Password did not conform with policy: ", string.Empty)
                        .Replace("Username should be an email", "Please enter valid email")));
        }

        throw new RpcException(new Status(StatusCode.Unknown, "Failed to confirm bind email"));
    }

    public override async Task<CardLevelUpReply> CardLevelUp(CardLevelUpRequest request, ServerCallContext context)
    {
        var playerId = context.GetPlayerId();
        var player = await _playerRepository.GetAsync(playerId);
        if (player == null)
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "No player data found"));

        var playerCard = player.Cards.FirstOrDefault(x => x.Id == request.CardId);
        if (playerCard == null)
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "Player card not found"));

        var cardData = _gameDataOption.CurrentValue.Cards.FirstOrDefault(x => x.Id == playerCard.Id);
        if (cardData == null)
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "Card data not found"));

        var gameData = _gameDataOption.CurrentValue;
        if (playerCard.Level >= gameData.PlayerProgressions[player.Level].CardLevelCap)
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "Card level cap reached"));

        var playerCardShards = player.CardShards.FirstOrDefault(x => x.Id == cardData.UpgradeShardId) ??
                               new Model.PlayerCardShards
                               {
                                   Id = cardData.UpgradeShardId,
                                   Amount = 0
                               };
        var cardProgression = gameData.CardProgressions[playerCard.Level];
        var cardShardsCost = cardProgression.LevelUpCost;
        var universalShardsCost = Math.Max(0, cardShardsCost - playerCardShards.Amount);

        if (cardShardsCost > playerCardShards.Amount && universalShardsCost > player.UniversalShards)
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "Not enough Shards"));

        playerCardShards.Amount = Math.Max(0, playerCardShards.Amount - cardShardsCost);
        player.UniversalShards = Math.Max(0, player.UniversalShards - universalShardsCost);
        player.Exp += cardProgression.PlayerExp;
        playerCard.Level++;

        var levelUpCost = gameData.PlayerProgressions[player.Level].LevelUpCost;
        var levelUp = player.Exp >= levelUpCost;
        if (levelUp && player.Level < gameData.PlayerProgressions.Count - 1)
        {
            player.Exp -= gameData.PlayerProgressions[player.Level].LevelUpCost;
            player.Level++;
        }

        await _playerRepository.UpdateAsync(player);

        return new CardLevelUpReply
        {
            Level = playerCard.Level,
            CardShards = playerCardShards.ToProto(),
            UniversalShards = player.UniversalShards,
            PlayerLevel = player.Level,
            PlayerExp = player.Exp
        };
    }

    public override async Task<ClearBattleRewardsReply> ClearBattleRewards(
        ClearBattleRewardsRequest request,
        ServerCallContext context)
    {
        var playerId = context.GetPlayerId();
        var player = await _playerRepository.GetAsync(playerId);
        if (player == null)
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "No player data found"));

        player.BattleRewards = Array.Empty<Model.PlayerBattleReward>();
        await _playerRepository.UpdateAsync(player);

        return new ClearBattleRewardsReply();
    }

    public override async Task<RedeemPromoCodeReply> RedeemPromoCode(RedeemPromoCodeRequest request, ServerCallContext context)
    {
        var playerId = context.GetPlayerId();
        var player = await _playerRepository.GetAsync(playerId);
        if (player == null)
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "No player data found"));

        var promoCode = request.PromoCode.ToLowerInvariant();
        var playerPromoCodes = player.PromoCodes ?? new List<string>();
        if (playerPromoCodes.Contains(promoCode))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Promo code already used"));

        if (promoCode == "CODE")
        {
            if (player.Level < 2)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid promo code"));

            playerPromoCodes.Add(promoCode);
            player.UniversalShards += 100;
            player.PromoCodes = playerPromoCodes;

            await _playerRepository.UpdateAsync(player);

            return new RedeemPromoCodeReply
            {
                Message = "You’ve received\n<sprite name=\"universal_shards\"> 100",
                Player = player.ToProto()
            };
        }

        if (promoCode == "CODE")
        {
            playerPromoCodes.Add(promoCode);
            player.UniversalShards += 50;
            player.PromoCodes = playerPromoCodes;

            await _playerRepository.UpdateAsync(player);

            return new RedeemPromoCodeReply
            {
                Message = "You’ve received\n<sprite name=\"universal_shards\"> 50",
                Player = player.ToProto()
            };
        }
        
        throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid promo code"));
    }

    public override async Task<BindImmutableWalletReply> BindImmutableWallet(
        BindImmutableWalletRequest request,
        ServerCallContext context)
    {
        var playerId = context.GetPlayerId();
        var player = await _playerRepository.GetAsync(playerId);
        if (player == null)
            throw new RpcException(new Status(StatusCode.FailedPrecondition, "No player data found"));

        if (player.AccountType != Model.AccountType.Standard)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Player is unable to bind Immutable Wallet"));

        var immutableUserInfo = await _immutableService.GetUserInfo(request.ImmutableToken);
        if (immutableUserInfo == null)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Immutable Passport token"));

        player.ImmutableWalletId = immutableUserInfo.passport_address;
        await _playerRepository.UpdateAsync(player);

        return new BindImmutableWalletReply
        {
            Player = player.ToProto()
        };
    }
}