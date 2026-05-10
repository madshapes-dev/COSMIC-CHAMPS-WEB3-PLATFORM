using UniRx;

namespace CosmicChamps.Data
{
    public class PlayerCard
    {
        public string Id { set; get; }
        public readonly IntReactiveProperty Level = new();
    }
}