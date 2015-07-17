using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Fiddler;

namespace Grabacr07.KanColleWrapper.Internal
{
	internal static class Extensions
	{
		public static string GetResponseAsJson(this Session session)
		{
			return session.GetResponseBodyAsString().Replace("svdata=", "");
		}

		/// <summary>
		/// <see cref="Int32" /> 型の配列に安全にアクセスします。
		/// </summary>
		public static int? Get(this int[] array, int index)
		{
			return array.Length > index ? (int?)array[index] : null;
		}

        public static T Get<T>(this T[] array, int index) where T : class
        {
            return array.Length > index ? array[index] : null;
        }

        public static T[] ConsolidateNonNull<T>(this T[] array) where T:class
        {
            for (int i = 0; i < array.Length; i++) {
                if (array[i] != null) continue;
                for (int j = i + 1; j < array.Length; j++) {
                    if (array[j] == null) continue;
                    array[i] = array[j];
                    array[j] = null;
                    goto NextLoop;
                }
                return array;
                NextLoop:;
            }
            return array;
        }

        public static string Join(this IEnumerable<string> values, string separator)
		{
			return string.Join(separator, values);
		}

		public static Task WhenAll(this IEnumerable<Task> tasks)
		{
			return Task.WhenAll(tasks);
		}

		public static Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> tasks)
		{
			return Task.WhenAll(tasks);
		}
	}
}
