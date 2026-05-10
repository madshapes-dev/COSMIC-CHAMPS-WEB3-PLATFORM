namespace CosmicChamps.Networking
{
    public class AuthData
    {
        public string PlayerId;
        public string PlayerSessionId;
        // public string Nickname;
        // public int DeckIndex;
        // public int Rating;

        protected bool Equals (AuthData other)
        {
            return PlayerId == other.PlayerId;
        }

        public override bool Equals (object obj)
        {
            if (ReferenceEquals (null, obj)) return false;
            if (ReferenceEquals (this, obj)) return true;
            if (obj.GetType () != this.GetType ()) return false;
            return Equals ((AuthData)obj);
        }

        public override int GetHashCode ()
        {
            return (PlayerId != null ? PlayerId.GetHashCode () : 0);
        }

        public override string ToString ()
        {
            return
                $"{nameof (PlayerId)}: {PlayerId}, {nameof (PlayerSessionId)}: {PlayerSessionId}";
        }
    }
}