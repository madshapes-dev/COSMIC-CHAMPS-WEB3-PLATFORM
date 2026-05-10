using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace ThirdParty.Extensions
{
    public static class ListExtensions
    {
        public static void Resize<T> (this List<T> list, int size, Func<T> elementFactory = null)
        {
            var count = list.Count;
            if (size == count)
                return;

            if (size < count)
            {
                list.RemoveRange (size, count - size);
                return;
            }

            if (size > list.Capacity)
                list.Capacity = size;

            for (var i = 0; i < size - count; i++)
            {
                list.Add (
                    elementFactory == null
                        ? default
                        : elementFactory ());
            }
        }

        public static List<List<T>> ChunkBy<T> (this IEnumerable<T> source, int chunkSize)
        {
            return source
                .Select ((x, i) => new { Index = i, Value = x })
                .GroupBy (x => x.Index / chunkSize)
                .Select (x => x.Select (v => v.Value).ToList ())
                .ToList ();
        }

        public static List<List<T1>> ChunkBy<T, T1> (this IEnumerable<T> source, int chunkSize) where T1 : class
        {
            return source
                .Select ((x, i) => new { Index = i, Value = x as T1 })
                .GroupBy (x => x.Index / chunkSize)
                .Select (x => x.Select (v => v.Value).ToList ())
                .ToList ();
        }

        public static void Shuffle<T> (this List<T> list, int rounds = 1)
        {
            for (var i = 0; i < rounds; i++)
            {
                list.Sort ((_, _) => Random.Range (-1, 1));
            }
        }
    }
}