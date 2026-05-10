using CosmicChamps.Data;

namespace CosmicChamps.Battle.Data.Server
{
    public class Player
    {
        private readonly CosmicChamps.Data.Player _player;

        public readonly PlayerTeam Team;
        public readonly Deck Deck;
        public readonly PlayerEnergy Energy;
        public bool Online;
        public string Emoji;
        public long EmojiSetTime;

        public string Id => _player.Id;
        public int Rating => _player.Rating;
        public string DisplayName => _player.DisplayName;
        public string ShipCard => _player.ShipSlot.Id;
        public int Level => _player.Level.Value;

        public Player (
            CosmicChamps.Data.Player player,
            PlayerTeam team,
            Deck deck,
            PlayerEnergy energy,
            bool online)
        {
            _player = player;

            Team = team;
            Deck = deck;
            Energy = energy;
            Online = online;
        }

        public UnitBoost GetBoost (string id) => _player.GetBoost (id);
        public PlayerCard GetPlayerCard (string id) => _player.GetCard (id);
    }
}