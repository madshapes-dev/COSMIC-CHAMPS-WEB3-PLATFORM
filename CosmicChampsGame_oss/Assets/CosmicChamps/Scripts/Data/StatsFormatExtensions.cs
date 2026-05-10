using UnityEngine;

namespace CosmicChamps.Data
{
    public static class StatsFormatExtensions
    {
        public static string FormatIntStat (this int stat, int? boostStat) =>
            boostStat is > 0
                ? $"{Mathf.RoundToInt (stat * (1f + boostStat.Value / 100f))} (+{boostStat}%)"
                : stat.ToString ();
    }
}