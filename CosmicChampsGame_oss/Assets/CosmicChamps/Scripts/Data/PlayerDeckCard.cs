using System;

namespace CosmicChamps.Data
{
    public class PlayerDeckCard
    {
        public string Id { set; get; }
        public string Skin { set; get; }

        public PlayerDeckCard Clone () => new()
        {
            Id = Id,
            Skin = Skin,
        };

        protected bool Equals (PlayerDeckCard other)
        {
            return Id == other.Id && Skin == other.Skin;
        }

        public override bool Equals (object obj)
        {
            if (ReferenceEquals (null, obj)) return false;
            if (ReferenceEquals (this, obj)) return true;
            if (obj.GetType () != this.GetType ()) return false;
            return Equals ((PlayerDeckCard)obj);
        }

        public override int GetHashCode ()
        {
            return HashCode.Combine (Id, Skin);
        }
    }
}