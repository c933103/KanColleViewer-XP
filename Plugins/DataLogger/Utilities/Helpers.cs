using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LynLogger
{
    public static class Helpers
    {
        public static readonly DateTimeOffset Epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);
        public static long UnixTimestamp => ToUnixTimestamp(DateTimeOffset.UtcNow);

        public static long ToUnixTimestamp(this DateTimeOffset dt)
        {
            return (long)dt.Subtract(Epoch).TotalSeconds;
        }

        public static DateTimeOffset FromUnixTimestamp(long ts)
        {
            return Epoch.AddSeconds(ts);
        }

        private const string Base32Charset = "0123456789ABCDEFGHJKLMNPQRSTVWXYZ";
        public static string Base32Encode(this byte[] buf)
        {
            ushort tmp = 0;
            int bits = 0;
            StringBuilder s = new StringBuilder(buf.Length * 8 / 5);
            for(int i = 0; i < buf.Length; i++) {
                tmp <<= 8;
                tmp |= buf[i];
                bits += 8;
                while(bits >= 5) {
                    s.Append(Base32Charset[tmp >> (bits - 5)]);
                    bits -= 5;
                    tmp <<= 16-bits;
                    tmp &= 0xFE00;
                    tmp >>= 16-bits;
                }
            }
            if(bits != 0) {
                s.Append(Base32Charset[tmp << (5 - bits)]);
                for(int i = 0; i < 5-bits; i++) {
                    s.Append(Base32Charset[32]);
                }
            }
            return s.ToString();
        }

        public static IEnumerable<TResult> SelectWithPrevious<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource, TResult> projection)
        {
            using(var enumerator = source.GetEnumerator()) {
                if(!enumerator.MoveNext()) yield break;
                var previous = enumerator.Current;
                while(enumerator.MoveNext()) {
                    yield return projection(previous, enumerator.Current);
                    previous = enumerator.Current;
                }
            }
        }

        public static string ToString(this double val, string format, int targetWidth = 8)
        {
            var text = val.ToString(format);
            if(text.Length > targetWidth) {
                var trimmed = val.ToString(format + "0");
                var decPlace = targetWidth - trimmed.Length - 1;
                if(decPlace < 0) decPlace = 0;
                text = val.ToString(format + decPlace);
            }
            return text;
        }

        public static IEnumerable<Tout> Zip<T1, T2, T3, T4, Tout>(IEnumerable<T1> i1, IEnumerable<T2> i2, IEnumerable<T3> i3, IEnumerable<T4> i4, Func<T1, T2, T3, T4, Tout> project)
        {
            using (var e1 = i1.GetEnumerator())
            using (var e2 = i2.GetEnumerator())
            using (var e3 = i3.GetEnumerator())
            using (var e4 = i4.GetEnumerator()) {
                System.Collections.IEnumerator[] enumerators = new System.Collections.IEnumerator[] { e1, e2, e3, e4 };
                while(enumerators.All(e => e.MoveNext())) {
                    yield return project(e1.Current, e2.Current, e3.Current, e4.Current);
                }
            }
        }

        public static IEnumerable<int> Sequence()
        {
            int x = 0;
            while(true) {
                yield return x++;
            }
        }

        /*public static IEnumerable<Tout> Zip<T1, T2, T3, Tout>(IEnumerable<T1> i1, IEnumerable<T2> i2, IEnumerable<T3> i3, Func<T1, T2, T3, Tout> project)
        {
            using (var e1 = i1.GetEnumerator())
            using (var e2 = i2.GetEnumerator())
            using (var e3 = i3.GetEnumerator()) {
                System.Collections.IEnumerator[] enumerators = new System.Collections.IEnumerator[] { e1, e2, e3 };
                while(enumerators.All(e => e.MoveNext())) {
                    yield return project(e1.Current, e2.Current, e3.Current);
                }
            }
        }*/

        public static IEnumerable<T> SafeConcat<T>(this IEnumerable<T> i1, params IEnumerable<T>[] i2)
        {
            IEnumerable<T> i = i1;
            int i2Index = 0;
            while(i == null && i2Index < i2.Length) {
                i = i2[i2Index];
                i2Index++;
            }

            for(; i2Index < i2.Length; i2Index++) {
                if(i2[i2Index] != null) {
                    i = i.Concat(i2[i2Index]);
                }
            }

            return i ?? new T[0];
        }

        public static IEnumerable<Tout> SafeExpand<Tin, Tout>(this IEnumerable<Tin> i, Func<Tin, IEnumerable<Tout>> project)
        {
            if(i == null) return new Tout[0];
            return i.SelectMany(project);
        }

        public static string GetEquiptNameWithFallback(int eid, string fallbackFormat = "{0} 号装备")
        {
            var  item = Grabacr07.KanColleWrapper.KanColleClient.Current.Master.SlotItems[eid];
            if(item == null) {
                return string.Format(fallbackFormat, eid);
            } else {
                return item.Name;
            }
        }

        public static T[] DeepCloneArray<T>(this T[] input)
        {
            if(input == null) return null;
            T[] output = new T[input.Length];
            for(int i = 0; i < input.Length; i++) {
                if(input[i] is Array && (input[i] as Array).Rank == 1) {
                    output[i] = (T)((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(input[i].GetType().GetElementType()).Invoke(null, new object[]{ input[i] });
                } else if(input[i] is ICloneable) {
                    output[i] = (T)((ICloneable)input[i]).Clone();
                } else {
                    output[i] = input[i];
                }
            }
            return output;
        }

        public static T[] ForEach<T>(this T[] arr, Action<T> act)
        {
            if(arr == null || act == null) return arr;
            for(int i = 0; i < arr.Length; i++) {
                act(arr[i]);
            }
            return arr;
        }

        public static T Clone<T>(this T s) where T :ICloneable
        {
            if (s == null) return default(T);
            return (T)s.Clone();
        }

        public static ulong ToLogbookSequence(this DateTimeOffset time)
        {
            time = time.UtcDateTime;
            return (ulong)(time.Year * 12 + (time.Month - 1));
        }

        public static DateTimeOffset FromLogbookSequence(ulong seq)
        {
            var year = (int)(seq / 12);
            var month = (int)(seq % 12);
            return new DateTimeOffset(year, month+1, 1, 0, 0, 0, 0, TimeSpan.Zero);
        }
    }
}
