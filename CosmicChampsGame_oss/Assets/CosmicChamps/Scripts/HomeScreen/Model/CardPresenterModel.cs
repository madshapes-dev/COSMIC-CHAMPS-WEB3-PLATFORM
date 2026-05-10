using CosmicChamps.Data;
using OneOf;

namespace CosmicChamps.HomeScreen.Model
{
    public class CardPresenterModel : OneOfBase<PlayerDeckCard, (PlayerCard, PlayerUnit)>
    {
        public static implicit operator CardPresenterModel (PlayerDeckCard _) => new(_);
        public static implicit operator CardPresenterModel ((PlayerCard, PlayerUnit) _) => new(_);

        public string Id => Match (deckCard => deckCard.Id, cardUnitPair => cardUnitPair.Item1.Id);
        public string Skin => Match (deckCard => deckCard.Skin, cardUnitPair => cardUnitPair.Item2.Skins[0]);

        public bool IsDeckCard => IsT0;
        public bool IsPlayerCard => IsT1;
        public PlayerDeckCard AsDeckCard => AsT0;
        public (PlayerCard, PlayerUnit) AsPlayerCard => AsT1;

        private CardPresenterModel (OneOf<PlayerDeckCard, (PlayerCard, PlayerUnit)> input) : base (input)
        {
        }
    }
}