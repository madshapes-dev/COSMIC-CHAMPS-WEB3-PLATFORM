namespace CosmicChamps.Battle
{
    public class UnityRealtimeSinceStartupTimeProvider : ITimeProvider
    {
        public float Time => UnityEngine.Time.realtimeSinceStartup;
    }
}