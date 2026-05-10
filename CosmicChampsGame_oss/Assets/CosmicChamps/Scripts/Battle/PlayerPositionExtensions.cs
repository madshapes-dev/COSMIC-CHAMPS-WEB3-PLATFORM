using System;
using CosmicChamps.Battle.Data;

namespace CosmicChamps.Battle
{
    public static class PlayerPositionExtensions
    {
        public static PlayerTeam GetOpposite (this PlayerTeam playerTeam) =>
            playerTeam switch
            {
                PlayerTeam.Undefined => PlayerTeam.Undefined,
                PlayerTeam.North => PlayerTeam.South,
                PlayerTeam.South => PlayerTeam.North,
                _ => throw new ArgumentOutOfRangeException (nameof (playerTeam), playerTeam, null)
            };
    }
}