using System;
using System.Collections.Generic;
using System.Linq;

namespace ThirdParty.Extensions
{
    public static class CollectionExtensions
    {
        private static readonly Random _random = new();

        public static bool GetRandom<T> (this ICollection<T> collection, out T random)
        {
            random = default;
            if (collection.Count == 0)
                return false;

            random = collection.ElementAt (_random.Next (0, collection.Count));
            return true;
        }
    }
}