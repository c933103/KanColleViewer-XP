using LynLogger.DataStore.Premitives;
using LynLogger.DataStore.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LynLogger.Utilities
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

        public static string GetEquiptNameWithFallback(int eid, string fallbackFormat = "{0} 号装备")
        {
            var  item = Grabacr07.KanColleWrapper.KanColleClient.Current.Master.SlotItems[eid];
            if(item == null) {
                return string.Format(fallbackFormat, eid);
            } else {
                return item.Name;
            }
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
