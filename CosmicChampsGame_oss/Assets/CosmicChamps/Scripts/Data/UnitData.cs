namespace CosmicChamps.Data
{
    public class UnitData
    {
        public string Id { set; get; }
        public string BoostId { set; get; }
        public IUnitStats Stats { set; get; }
        public UnitViewParams ViewParams { set; get; }
        public UnitMovementType MovementType { set; get; }
        public string SpawnArea { set; get; }
        public bool Disabled { set; get; }
        public string Type { set; get; }

        public UnitData Clone (string id) => new()
        {
            Id = id,
            BoostId = BoostId,
            Stats = Stats.Clone (),
            ViewParams = ViewParams,
            MovementType = MovementType,
            SpawnArea = SpawnArea,
            Type = Type
        };
    }
}