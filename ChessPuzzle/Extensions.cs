using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessPuzzle
{
    static class Extensions
    {
        public static T? FirstOrNull<T>(this IEnumerable<T> seq) where T : struct
        {
            using var enumerator = seq.GetEnumerator();
            return enumerator.MoveNext() ? enumerator.Current : (T?)null;
        }

        public static T? FirstOrNull<T>(this IEnumerable<T> seq, Func<T, bool> predicate) where T : struct
            => seq.Where(predicate).FirstOrNull();
    }
}
