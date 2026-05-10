using System;
using System.Linq;

namespace CosmicChamps.Data
{
    public class PlayerDeck
    {
        public string Id { private set; get; }
        public string PresetId { set; get; }
        public PlayerDeckCard[] Cards { set; get; }

        public PlayerDeck ()
        {
            Id = Guid.NewGuid ().ToString ();
        }

        public PlayerDeck Clone () => new()
        {
            Id = Id,
            PresetId = PresetId,
            Cards = Array.ConvertAll (Cards, x => x?.Clone ())
        };

        public int? GetFreeSlot ()
        {
            var slot = Array.FindIndex (Cards, x => x == null);
            return slot < 0 ? null : slot;
        }

        public void RemoveCard (PlayerDeckCard card)
        {
            var slot = Array.IndexOf (Cards, card);
            if (slot < 0)
                throw new ArgumentException ("Card not found");

            Cards[slot] = null;
        }

        protected bool Equals (PlayerDeck other)
        {
            return Id == other.Id && PresetId == other.PresetId && Cards.SequenceEqual (other.Cards);
        }

        public override bool Equals (object obj)
        {
            if (ReferenceEquals (null, obj)) return false;
            if (ReferenceEquals (this, obj)) return true;
            if (obj.GetType () != this.GetType ()) return false;
            return Equals ((PlayerDeck)obj);
        }

        public override int GetHashCode ()
        {
            var hashCode = new HashCode ();
            hashCode.Add (Id);
            hashCode.Add (PresetId);
            foreach (var slot in Cards)
            {
                hashCode.Add (slot);
            }

            return hashCode.ToHashCode ();
        }
    }
}