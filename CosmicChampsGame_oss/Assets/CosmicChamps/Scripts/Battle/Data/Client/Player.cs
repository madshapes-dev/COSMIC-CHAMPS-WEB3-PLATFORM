using UniRx;

namespace CosmicChamps.Battle.Data.Client
{
    public class Player
    {
        public readonly string Id;
        public readonly PlayerTeam Team;
        public readonly Deck Deck;
        public readonly PlayerEnergy Energy;
        public readonly StringReactiveProperty Emoji = new();

        public Player (string id, PlayerTeam team, Deck deck, PlayerEnergy energy)
        {
            Id = id;
            Team = team;
            Deck = deck;
            Energy = energy;
        }
    }
}