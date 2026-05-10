using System;

namespace CosmicChamps.Battle.Data
{
    public class Card
    {
        public string Id;
        public string Skin;

        protected bool Equals (Card other)
        {
            return Id == other.Id && Skin == other.Skin;
        }

        public override bool Equals (object obj)
        {
            if (ReferenceEquals (null, obj)) return false;
            if (ReferenceEquals (this, obj)) return true;
            if (obj.GetType () != this.GetType ()) return false;
            return Equals ((Card)obj);
        }

        public override int GetHashCode ()
        {
            return HashCode.Combine (Id, Skin);
        }
    }
}