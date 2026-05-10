namespace CosmicChamps.Signals
{
    public class NetworkReachabilitySignal
    {
        public static readonly NetworkReachabilitySignal ReachableSignal = new(true);
        public static readonly NetworkReachabilitySignal UnreachableSignal = new(false);

        public readonly bool Reachable;

        public NetworkReachabilitySignal (bool reachable)
        {
            Reachable = reachable;
        }
    }
}