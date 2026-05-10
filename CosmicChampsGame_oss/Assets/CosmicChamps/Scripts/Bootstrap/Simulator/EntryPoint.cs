using System;
using CosmicChamps.Services;
using UnityEngine;
using Zenject;
using ILogger = Serilog.ILogger;

namespace CosmicChamps.Bootstrap.Simulator
{
    public class EntryPoint : MonoBehaviour
    {
        [Inject]
        private IGameService _gameService;

        [Inject]
        private ILogger _logger;

        [Inject]
        private SimulatorNetworkService _networkService;

        private async void Start ()
        {
            Application.runInBackground = true;
            Application.targetFrameRate = 60;
            
            await _gameService.LoadGameData ();
            await _gameService.LoadPlayerData ();

            _logger.Information ("Data successfuly requested");

            try
            {
                var enumerator = _gameService.StartMatchmaking (false).GetAsyncEnumerator ();
                while (await enumerator.MoveNextAsync ())
                {
                    var matchMakingResult = enumerator.Current;
                    matchMakingResult.Switch (
                        playerGameSession =>
                        {
                            _logger.Information ("matchMakingResult");
                            _networkService.Start (playerGameSession);
                        },
                        _ => { _logger.Information ("matchmakingTicket"); },
                        _ => { _logger.Information ("matchmakingTimeout"); },
                        _ => _logger.Information ("matchmakingCancellation"));
                }
            } catch (Exception e)
            {
                _logger.Error ("Failed to start matchmaking: {Message}", e.Message);
                Application.Quit (1);
                throw;
            }
        }
    }
}