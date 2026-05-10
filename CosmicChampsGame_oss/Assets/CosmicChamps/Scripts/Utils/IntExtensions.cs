using System.Globalization;

namespace CosmicChamps.Utils
{
    public static class IntExtensions
    {
        public static string FormatShardsCost (this int cost) =>
            cost < 1000 ? cost.ToString () : $"{(cost / 1000f).ToString ("F1", CultureInfo.InvariantCulture)}k";
    }
}