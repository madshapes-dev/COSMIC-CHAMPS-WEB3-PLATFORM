using UniRx;

namespace CosmicChamps.Battle.Data.Client
{
    public class Opponent
    {
        public readonly string Id;
        public readonly string Name;
        public readonly int Level;
        public readonly int Rating;
        public readonly StringReactiveProperty Emoji = new();

        public Opponent (string id, string name, int level, int rating)
        {
            Id = id;
            Name = name;
            Rating = rating;
            Level = level;
        }
    }
}