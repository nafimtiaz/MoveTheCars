using System.Collections.Generic;
using System.Linq;

namespace MTC.Utils
{
    public static class CollectionExtensions
    {
        public static List<T> GetShuffledList<T>(this IList<T> ls)
        {
            var count = ls.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i) {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = ls[i];
                ls[i] = ls[r];
                ls[r] = tmp;
            }

            return ls.ToList();
        }
    }
}