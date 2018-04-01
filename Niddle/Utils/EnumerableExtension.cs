using System.Collections.Generic;

namespace Niddle.Utils
{
    static public class EnumerableExtension
    {
        static public int IndexOf<T>(this IEnumerable<T> enumerable, T value)
        {
            int index = 0;
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            foreach (T item in enumerable)
            {
                if (comparer.Equals(item, value))
                    return index;
                index++;
            }
            return -1;
        }
    }
}