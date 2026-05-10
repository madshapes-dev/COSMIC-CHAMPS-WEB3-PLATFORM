using CosmicChamps.Data;
using OneOf;

namespace CosmicChamps.HomeScreen.Model
{
    public class ShipCardPresenterModel : OneOfBase<PlayerShipSlotCard, PlayerShipCard>
    {
        public static implicit operator ShipCardPresenterModel (PlayerShipSlotCard _) => new(_);
        public static implicit operator ShipCardPresenterModel (PlayerShipCard _) => new(_);

        public string Id => Match (deckCard => deckCard.Id, playerCard => playerCard.Id);
        public string Skin => Match (deckCard => deckCard.Skin, playerCard => playerCard.Skins[0]);

        public bool IsDeckCard => IsT0;
        public bool IsPlayerCard => IsT1;
        public PlayerShipSlotCard AsDeckCard => AsT0;
        public PlayerShipCard AsPlayerCard => AsT1;

        protected ShipCardPresenterModel (OneOf<PlayerShipSlotCard, PlayerShipCard> input) : base (input)
        {
        }
    }
}