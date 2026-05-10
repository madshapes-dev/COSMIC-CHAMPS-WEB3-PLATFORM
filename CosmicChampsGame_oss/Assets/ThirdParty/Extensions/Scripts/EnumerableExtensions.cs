using System;
using System.Collections.Generic;

namespace ThirdParty.Extensions
{
    public static class EnumerableExtensions
    {
        public static int IndexOf<T> (this IEnumerable<T> enumerable, T item)
        {
            if (enumerable == null)
                throw new ArgumentException ("Enumerable cannot be null");

            if (item == null)
                throw new ArgumentException ("Item cannot be null");

            var i = 0;
            using var enumerator = enumerable.GetEnumerator ();
            while (enumerator.MoveNext ())
            {
                if (item.Equals (enumerator.Current))
                    return i;

                i++;
            }

            return -1;
        }

        public static int IndexOf<T> (this IEnumerable<T> enumerable, Predicate<T> predicate)
        {
            if (enumerable == null)
                throw new ArgumentException ("Enumerable cannot be null");

            var i = 0;
            using var enumerator = enumerable.GetEnumerator ();
            while (enumerator.MoveNext ())
            {
                if (predicate (enumerator.Current))
                    return i;

                i++;
            }

            return -1;
        }
    }
}