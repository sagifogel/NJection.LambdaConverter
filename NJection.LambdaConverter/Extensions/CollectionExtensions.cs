using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NJection.LambdaConverter.Extensions
{
    public static class CollectionExtensions
    {
        public static void ForEach<TSource>(this IEnumerable<TSource> source, int seed, Action<TSource, int> action) {
            foreach (TSource item in source) {
                action(item, seed++);
            }
        }

        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource, int> action) {
            int count = 0;

            foreach (TSource item in source) {
                action(item, count++);
            }
        }

        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action) {
            foreach (TSource item in source) {
                action(item);
            }
        }

        public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, int, bool> predicate) {
            int count = 0;

            foreach (TSource local in source) {
                if (!predicate(local, count++)) {
                    return false;
                }
            }

            return true;
        }

        public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> source) {
            if (source == null) {
                return true;
            }

            return !source.Any();
        }

		public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> source, params TSource[] others) {
            foreach (TSource item in source) {
                yield return item;
            }

            foreach (TSource item in others) {
                yield return item;
            }
        }

        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> predicate) {
            HashSet<TKey> keys = new HashSet<TKey>();

            foreach (TSource element in source) {
                if (keys.Add(predicate(element))) {
                    yield return element;
                }
            }
        }

        public static TResult[] ToArrayOf<TResult>(this IEnumerable source) {
            return source.Cast<TResult>()
                         .ToArray();
        }

        public static List<TResult> ToListOf<TResult>(this IEnumerable source) {
            return source.Cast<TResult>()
                         .ToList();
        }

        public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
            int index = -1;
            int count = -1;

            foreach (var item in source) {
                count++;

                if (predicate(item)) {
                    index = count;
                    break;
                }
            }

            return index;
        }

        public static ConcurrentDictionary<TKey, TSource> ToThreadSafeDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) {
            return ToThreadSafeDictionary(source, local => keySelector(local), local => local);
        }

        public static ConcurrentDictionary<TKey, TValue> ToThreadSafeDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> elementSelector) {
            return new ConcurrentDictionary<TKey, TValue>(
                    source.ToDictionary(local => keySelector(local), local => elementSelector(local)));
        }
    }
}