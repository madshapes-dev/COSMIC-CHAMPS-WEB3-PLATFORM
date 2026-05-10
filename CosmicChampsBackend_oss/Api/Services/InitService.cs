using CosmicChamps.Api.Model;
using Microsoft.Extensions.Options;

namespace CosmicChamps.Api.Services;

public class InitService
{
    private readonly IOptionsMonitor<GameData> _gameDataOption;
    private readonly IPlayerRepository _playerRepository;

    public InitService (IOptionsMonitor<GameData> gameDataOption, IPlayerRepository playerRepository)
    {
        _gameDataOption = gameDataOption;
        _playerRepository = playerRepository;
    }

    /*private async Task CreateBot (Bot bot)
    {
        int rating;

        var gameData = _gameDataOption.CurrentValue;
        var botPlayer = await _playerRepository.GetAsync (bot.PlayerId);
        if (botPlayer != null)
        {
            botPlayer.LinkedWalletId = bot.WalletId;
            botPlayer.Email = bot.Email;
            botPlayer.Nickname = bot.Nickname;
            botPlayer.Rating = int.TryParse (bot.Data[BotData.Rating], out rating) ? rating : 1200;

            await _playerRepository.UpdateAsync (botPlayer);

            return;
        }

        botPlayer = new Player
        {
            Id = bot.PlayerId,
            WalletId = bot.WalletId,
            LinkedWalletId = bot.WalletId,
            Email = bot.Email,
            Nickname = bot.Nickname,
            Units = gameData.Units.Select (
                    x => new Model.PlayerUnit
                    {
                        Id = x.Id,
                        Skins = x.Aliases.Prepend (x.Id).ToArray ()
                    })
                .ToArray (),
            Cards = gameData.Cards.Select (
                    x => new Model.PlayerCard
                    {
                        Id = x.Id,
                        Level = 0
                    })
                .ToArray (),
            Rating = int.TryParse (bot.Data[BotData.Rating], out rating) ? rating : 1200,
            Decks = _gameDataOption
                .CurrentValue
                .DeckPresets
                .Select (
                    x => new Model.PlayerDeck
                    {
                        PresetId = x.Id,
                        Cards = x
                            .Cards
                            .Select (y => y == null ? null : new Model.PlayerDeckCard { Id = y.Id, UnitSkin = y.Skin })
                            .ToArray ()
                    })
                .ToArray (),
            ActiveDeckIndex = 0,
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
            SignUpCompleted = true,
            Emojis = new[] { "ram", "coy", "inv" },
            IsGuest = false,
            UniversalShards = 0,
            CardShards = Array.Empty<Model.PlayerCardShards> (),
            Level = 0,
            Exp = 0
        };

        await _playerRepository.CreateAsync (botPlayer);
    }

    private async Task CreateBots ()
    {
        var gameData = _gameDataOption.CurrentValue;

        foreach (var bot in gameData.Bots)
        {
            await CreateBot (bot);
        }
    }*/

    public async Task Run ()
    {
        // await CreateBots ();
    }
}