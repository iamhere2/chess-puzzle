using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ChessPuzzle
{
    static class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? FirstOrNull<T>(this IEnumerable<T> seq) where T : struct
        {
            using var enumerator = seq.GetEnumerator();
            return enumerator.MoveNext() ? enumerator.Current : (T?)null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? FirstOrNull<T>(this IEnumerable<T> seq, Func<T, bool> predicate) where T : struct
            => seq.Where(predicate).FirstOrNull();
    }
}
