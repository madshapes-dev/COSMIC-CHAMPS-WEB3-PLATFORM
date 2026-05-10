namespace CosmicChamps.Data
{
    public class DeckPreset
    {
        public string Id { set; get; }
        public string Name { set; get; }
        public DeckPresetCard[] Cards { set; get; }
    }
}