namespace CosmicChamps.Battle.Data.Client
{
    public class Battle
    {
        public readonly int MatchDuration;

        public float? OvertimeDuration { private set; get; }

        public float Duration { private set; get; } // Current duration
        public float DurationTimestamp { private set; get; } // Time when current duration data arrived

        public Battle (float duration, float durationTimestamp, int matchDuration, float? overtimeDuration)
        {
            Duration = duration;
            OvertimeDuration = null;
            DurationTimestamp = durationTimestamp;
            MatchDuration = matchDuration;
            OvertimeDuration = overtimeDuration;
        }

        public void AdjustDuration (float duration, float durationTimestamp, float? overtimeDuration)
        {
            Duration = duration;
            DurationTimestamp = durationTimestamp;
            OvertimeDuration = overtimeDuration;
        }
    }
}