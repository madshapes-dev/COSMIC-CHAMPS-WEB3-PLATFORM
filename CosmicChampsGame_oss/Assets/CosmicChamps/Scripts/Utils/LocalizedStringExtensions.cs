using System.Linq;
using UnityEngine.Localization;

namespace CosmicChamps.Utils
{
    public static class LocalizedStringExtensions
    {
        public static string Format (this LocalizedString str, params (string, object)[] args)
        {
            str.Arguments = new object[] { args.ToDictionary (x => x.Item1, x => x.Item2) };
            return str.GetLocalizedString ();
        }
    }
}