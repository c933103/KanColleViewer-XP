using System;
using System.Collections.Generic;
using System.Linq;

namespace Grabacr07.KanColleWrapper
{
	public static class EnumerableEx
	{
		public static IEnumerable<TResult> Return<TResult>(TResult value)
		{
			yield return value;
		}

		public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			var set = new HashSet<TKey>(EqualityComparer<TKey>.Default);

			foreach (var item in source)
			{
				var key = keySelector(item);
				if (set.Add(key)) yield return item;
			}
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            int i = 0;
            foreach (var obj in source) {
                action(obj, i);
                i++;
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var obj in source) {
                action(obj);
            }
        }

        public static SortedList<TKey, TValue> ToSortedList<TKey, TValue>(this IEnumerable<TValue> source, Func<TValue, TKey> keySelector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            var ret = new SortedList<TKey, TValue>();
            foreach(var val in source.OrderBy(keySelector)) {
                ret.Add(keySelector(val), val);
            }

            return ret;
        }

        public static int BinarySearchEqualOrHigher<T>(this IList<T> list, T value, IComparer<T> comparer = null)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (list.Count == 0) return 0;

            comparer = comparer ?? Comparer<T>.Default;
            var left = 0;
            var right = list.Count - 1;

            while (left != right) {
                var index = (int)(((uint)left + (uint)right) / 2);
                var compare = comparer.Compare(list[index], value);
                if (compare == 0) return index;
                if (compare < 0) {
                    left = index + 1;
                } else {
                    right = index - 1;
                }
            }
            if (comparer.Compare(list[right], value) < 0) return right + 1;
            return right;
        }

        /// <summary>
        /// コレクションを展開し、メンバーの文字列表現を指定した区切り文字で連結した文字列を返します。
        /// </summary>
        /// <typeparam name="T">コレクションに含まれる任意の型。</typeparam>
        /// <param name="source">対象のコレクション。</param>
        /// <param name="separator">セパレーターとして使用する文字列。</param>
        /// <returns>コレクションの文字列表現を展開し、指定したセパレーターで連結した文字列。</returns>
        public static string ToString<T>(this IEnumerable<T> source, string separator)
		{
			return string.Join(separator, source);
		}

		/// <summary>
		/// シーケンスが null でなく、1 つ以上の要素を含んでいるかどうかを確認します。
		/// </summary>
		public static bool HasItems<T>(this IEnumerable<T> source)
		{
			return source != null && source.Any();
		}
	}
}
