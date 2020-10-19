using System.Collections.Generic;
using System.Linq;

namespace PreheatingStationControl.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> SubArray<T>(this IEnumerable<T> value, int startIndex, int count)
        {
            return value.Skip(startIndex).Take(count);
        }
    }
}
