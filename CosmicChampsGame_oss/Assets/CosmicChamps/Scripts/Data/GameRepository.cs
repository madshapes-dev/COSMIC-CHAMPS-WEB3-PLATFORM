using System;
using System.Collections.Generic;
using System.Linq;
using CosmicChamps.Api.Services;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;

namespace CosmicChamps.Data
{
    public class GameRepository
    {
        private readonly Game.GameClient _gameClient;
        private readonly IDeviceIdProvider _deviceIdProvider;

        public GameRepository (Game.GameClient gameClient, IDeviceIdProvider deviceIdProvider)
        {
            _gameClient = gameClient;
            _deviceIdProvider = deviceIdProvider;
        }

        public IUniTaskAsyncEnumerable<StartGameResult> StartMatchmaking (string tournamentId = null)
        {
            return UniTaskAsyncEnumerable.Create<StartGameResult> (
                async (writer, cancellationToken) =>
                {
                    var responseStream = _gameClient.StartMatchmaking (
                            new StartMatchmakingRequest
                            {
                                TournamentId = tournamentId ?? string.Empty
                            })
                        .ResponseStream;

                    while (await responseStream.MoveNext (cancellationToken))
                    {
                        var startGameReply = responseStream.Current;
                        switch (startGameReply.ResultCase)
                        {
                            case StartMatchmakingReply.ResultOneofCase.GameSession:
                                var gameSession = startGameReply.GameSession;
                                await writer.YieldAsync (
                                    new PlayerGameSession
                                    {
                                        Id = gameSession.GameSessionId,
                                        DnsName = gameSession.DnsName,
                                        Port = gameSession.Port,
                                        WebGLPort = gameSession.WebGLPort,
                                        PlayerSessionId = gameSession.PlayerSessionId,
                                        IpAddress = gameSession.IpAddress
                                    });
                                return;
                            case StartMatchmakingReply.ResultOneofCase.Timeout:
                                await writer.YieldAsync (new MatchmakingTimeout ());
                                break;
                            case StartMatchmakingReply.ResultOneofCase.TicketId:
                                await writer.YieldAsync (new MatchmakingTicket { Id = startGameReply.TicketId });
                                break;
                            case StartMatchmakingReply.ResultOneofCase.Canceled:
                                await writer.YieldAsync (new MatchmakingCancellation ());
                                return;
                            case StartMatchmakingReply.ResultOneofCase.None:
                            default:
                                throw new ArgumentOutOfRangeException ();
                        }
                    }
                });
        }

        public UniTask StopMatchmaking (string ticketId) =>
            _gameClient.StopMatchmakingAsync (
                    new StopMatchmakingRequest
                    {
                        TicketId = ticketId
                    })
                .ResponseAsync
                .AsUniTask ();

        public async UniTask<(ICollection<Player> players, string level)> StartGameSession (
            GameSession gameSession,
            ICollection<string> playerIds,
            string matchmakingConfigurationArn) => await _gameClient
            //
            .StartGameSessionAsync (
                new StartGameSessionRequest
                {
                    GameSession = new Api.Services.GameSession
                    {
                        Id = gameSession.Id,
                        IpAddress = gameSession.IpAddress,
                        Port = gameSession.Port,
                        DnsName = gameSession.DnsName,
                        PlayerIds = { playerIds },
                        TournamentId = gameSession.TournamentId ?? string.Empty,
                        MatchmakerConfigurationArn = matchmakingConfigurationArn
                    }
                })
            .ResponseAsync
            .ContinueWith (x => (x.Result.Players.Select (y => y.ToModel ()).ToList (), x.Result.Level));

        public async UniTask StopGameSession (string gameSessionId, bool draw, string winnerId, string winnerWalletId) =>
            await _gameClient.StopGameSessionAsync (
                new StopGameSessionRequest
                {
                    GameSessionId = gameSessionId,
                    Drawn = draw,
                    WinnerId = winnerId ?? string.Empty,
                    WinnerWalletId = winnerWalletId ?? string.Empty
                });


        public async UniTask UpdateDecks (int activeDeckIndex, IEnumerable<(int index, PlayerDeck deck)> decks) =>
            await _gameClient.UpdateDecksAsync (
                new UpdateDecksRequest
                {
                    ActiveDeckIndex = activeDeckIndex,
                    DeckUpdates = { decks.Select (x => x.deck.ToProtoDeckUpdate (x.index)) }
                });

        public async UniTask<News> GetNews () => (await _gameClient.GetNewsAsync (new NewsRequest ()))
            .News
            .ToModel ();

        public async UniTask CompleteSignUp (string nickname) => await _gameClient.CompleteSignUpAsync (
            new CompleteSignUpRequest
            {
                Nickname = nickname ?? string.Empty
            });

        public async UniTask<string> ReportError (
            string version,
            string platform,
            string date,
            string gameSession,
            string message,
            string stacktrace) =>
            //
            (await _gameClient.ReportErrorAsync (
                new ReportErrorRequest
                {
                    Version = version,
                    Platform = platform,
                    Date = date,
                    GameSession = gameSession,
                    Message = message,
                    Stacktrace = stacktrace,
                    Device = new Device
                    {
                        Id = _deviceIdProvider.DeviceId,
                        Model = SystemInfo.deviceModel,
                        Graphics = new DeviceGraphics
                        {
                            Id = SystemInfo.graphicsDeviceID,
                            Name = SystemInfo.graphicsDeviceName,
                            Type = SystemInfo.graphicsDeviceType.ToString (),
                            Vendor = SystemInfo.graphicsDeviceVendor,
                            VendorID = SystemInfo.graphicsDeviceVendorID,
                            Version = SystemInfo.graphicsDeviceVersion,
                            MemorySize = SystemInfo.graphicsMemorySize
                        },
                        OperatingSystem = new DeviceOperatingSystem
                        {
                            Name = SystemInfo.operatingSystem,
                            Family = SystemInfo.operatingSystemFamily.ToString ()
                        }
                    }
                })).ReportId;

        public async UniTask<int> SaveProfile (string nickname) =>
            (await _gameClient.SaveProfileAsync (
                new SaveProfileRequest
                {
                    NicknameValue = nickname ?? string.Empty
                })).NicknameChangeCount;

        public UniTask<CardLevelUpResult> CardLevelUp (string cardId) =>
            _gameClient
                .CardLevelUpAsync (
                    new CardLevelUpRequest
                    {
                        CardId = cardId
                    })
                .ResponseAsync
                .ContinueWith (
                    x => new CardLevelUpResult (
                        x.Result.Level,
                        x.Result.CardShards.ToModel (),
                        x.Result.UniversalShards,
                        x.Result.PlayerLevel,
                        x.Result.PlayerExp))
                .AsUniTask ();

        public UniTask ClearBattleRewards () =>
            _gameClient
                .ClearBattleRewardsAsync (new ClearBattleRewardsRequest ())
                .ResponseAsync
                .AsUniTask ();
    }
}