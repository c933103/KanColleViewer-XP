using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Utilities
{
    public static class EnumerablesEx
    {
        public static int? SumNaT(this IEnumerable<int?> source)
        {
            int acc = 0;
            if(source == null) return null;
            foreach(var val in source) {
                if(!val.HasValue) return null;
                acc += val.Value;
            }
            return acc;
        }

        public static int? SumNaT<T>(this IEnumerable<T> source, Func<T, int?> selector)
        {
            int acc = 0;
            if(source == null) return null;
            foreach(var ele in source) {
                var val = selector?.Invoke(ele);
                if(!val.HasValue) return null;
                acc += val.Value;
            }
            return acc;
        }

        public static IEnumerable<TResult> SelectWithPrevious<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource, TResult> projection)
        {
            using (var enumerator = source.GetEnumerator()) {
                if (!enumerator.MoveNext()) yield break;
                var previous = enumerator.Current;
                while (enumerator.MoveNext()) {
                    yield return projection(previous, enumerator.Current);
                    previous = enumerator.Current;
                }
            }
        }

        public static IEnumerable<Tout> Zip<T1, T2, T3, T4, Tout>(IEnumerable<T1> i1, IEnumerable<T2> i2, IEnumerable<T3> i3, IEnumerable<T4> i4, Func<T1, T2, T3, T4, Tout> project)
        {
            if(i1 == null || i2 == null || i3 == null || i4 == null) yield break;

            using (var e1 = i1.GetEnumerator())
            using (var e2 = i2.GetEnumerator())
            using (var e3 = i3.GetEnumerator())
            using (var e4 = i4.GetEnumerator()) {
                System.Collections.IEnumerator[] enumerators = new System.Collections.IEnumerator[] { e1, e2, e3, e4 };
                while (enumerators.All(e => e.MoveNext())) {
                    yield return project(e1.Current, e2.Current, e3.Current, e4.Current);
                }
            }
        }

        public static IEnumerable<int> Sequence(int start = 0)
        {
            while (true) {
                yield return start++;
            }
        }

        public static IEnumerable<int> Sequence(int start, int? count)
        {
            if(count.HasValue) return Sequence(start, count.Value);
            return null;
        }

        public static IEnumerable<int> Sequence(int start, int count)
        {
            for (int i = 0; i < count; i++) {
                yield return start++;
            }
        }

        public static IEnumerable<int> Range(int start, int end)
        {
            while (start < end) {
                yield return start++;
            }
        }

        public static IEnumerable<T> SafeConcat<T>(this IEnumerable<T> i1, params IEnumerable<T>[] i2)
        {
            IEnumerable<T> i = i1;
            int i2Index = 0;
            while (i == null && i2Index < i2.Length) {
                i = i2[i2Index];
                i2Index++;
            }

            for (; i2Index < i2.Length; i2Index++) {
                if (i2[i2Index] != null) {
                    i = i.Concat(i2[i2Index]);
                }
            }

            return i ?? new T[0];
        }

        public static IEnumerable<Tout> SafeExpand<Tin, Tout>(this IEnumerable<Tin> i, Func<Tin, IEnumerable<Tout>> project)
        {
            if (i == null) return new Tout[0];
            return i.SelectMany(project);
        }

        public static T[] DeepCloneArray<T>(this T[] input)
        {
            if (input == null) return null;
            T[] output = new T[input.Length];
            for (int i = 0; i < input.Length; i++) {
                if (input[i] is Array && (input[i] as Array).Rank == 1) {
                    output[i] = (T)((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(input[i].GetType().GetElementType()).Invoke(null, new object[] { input[i] });
                } else if (input[i] is ICloneable) {
                    output[i] = (T)((ICloneable)input[i]).Clone();
                } else {
                    output[i] = input[i];
                }
            }
            return output;
        }

        public static T[] ForEach<T>(this T[] arr, Action<T> act)
        {
            if (arr == null || act == null) return arr;
            for (int i = 0; i < arr.Length; i++) {
                act(arr[i]);
            }
            return arr;
        }

        public static IEnumerable<T> AsEnumerable<T>(params T[] ts) => ts;

        public static T Get<T>(this T[] arr, int i) { return arr[i]; }

        public static V GetWithFallback<K, V>(this IReadOnlyDictionary<K, V> dict, K key, V fallback)
        {
            V res;
            if (dict.TryGetValue(key, out res)) return res;
            return fallback;
        }

        public static T GetWithFallback<T>(this T[] dict, int idx, T fallback)
        {
            if (idx < dict.Length) return dict[idx];
            return fallback;
        }
    }
}
