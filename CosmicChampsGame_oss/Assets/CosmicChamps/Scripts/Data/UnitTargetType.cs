using System;

namespace CosmicChamps.Data
{
    [Flags]
    public enum UnitTargetType
    {
        None = 0,
        Base = 1,
        Ground = 2,
        Air = 4,
        Spell = 8
    }
}