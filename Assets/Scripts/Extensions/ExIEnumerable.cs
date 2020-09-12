using System;
using System.Collections.Generic;

namespace Assets.Scripts.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IEnumerable{T}" />.
    /// </summary>
    public static class ExIEnumerable
    {
        /// <summary>
        /// Performs the specified actions with each collection item.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="source">The source collection.</param>
        /// <param name="actions">The actions.</param>
        public static void ForEach<T>(this IEnumerable<T> source, params Action<T>[] actions)
        {
            var enumerator = source?.GetEnumerator() ?? throw new NullReferenceException(nameof(source));

            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;

                if (item == null)
                    continue;

                for (var i = 0; i < actions.Length; i++)
                    actions[i]?.Invoke(item);
            }
        }
    }
}
