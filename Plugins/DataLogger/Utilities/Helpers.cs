using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LynLogger
{
    public static class Helpers
    {
        public static readonly DateTimeOffset Epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);
        public static long UnixTimestamp { get { return ToUnixTimestamp(DateTimeOffset.UtcNow); } }

        public static long ToUnixTimestamp(DateTimeOffset dt)
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

        public static void WriteVLCI(this Stream output, ulong val)
        {
            for(int i = 0; i < 9; i++) {
                byte thisByte = (byte)(val & 0x7F);
                val >>= 7;
                if(val != 0) {
                    thisByte |= 0x80;
                    output.WriteByte(thisByte);
                } else {
                    output.WriteByte(thisByte);
                    break;
                }
            }
        }

        public static ulong ReadVLCI(this Stream input)
        {
            ulong val = 0;
            for(int i = 0; i < 9; i++) {
                int b = input.ReadByte();
                if(b < 0)
                    throw new EndOfStreamException();

                ulong l = (ulong)(b & 0x7F);
                val |= l << (7*i);

                if((b & 0x80) == 0) break;
                if(i == 8) {
                    if((b & 0x80) != 0) val |= 1UL << 63;
                }
            }

            return val;
        }

        private static readonly byte[] _blankByteArray = new byte[0];
        private const int maxDictLogSize = 24;
        public static void CompressData(Stream input, byte[] training, Stream output, int lp = 0, int lc = 3, int pb = 2, int fb = 32, string mf = "BT4")
        {
            if(training == null) training = _blankByteArray;
            using(MemoryStream train = new MemoryStream(training, false)) {
                using(SevenZip.DoubleStream stream = new SevenZip.DoubleStream()) {
                    stream.s1 = train;
                    stream.s2 = input;
                    stream.fileIndex = 0;
                    stream.skipSize = 0;

                    int dictLogSize;
                    bool eos;
                    long dataLen;
                    if(input.CanSeek) {
                        dataLen = input.Length;
                        var totalLen = training.Length + dataLen;
                        for(dictLogSize = 10; dictLogSize < maxDictLogSize; dictLogSize++) {
                            if(totalLen < (1 << dictLogSize)) break;
                        }
                        eos = false;
                    } else {
                        dataLen = -1;
                        dictLogSize = maxDictLogSize;
                        eos = true;
                    }
                    int dictSize = 1 << dictLogSize;

                    if(train.Length > dictSize) stream.skipSize = train.Length - dictSize;
                    train.Seek(stream.skipSize, SeekOrigin.Begin);

                    SevenZip.Compression.LZMA.Encoder comp = new SevenZip.Compression.LZMA.Encoder();
                    comp.SetCoderProperties(new SevenZip.CoderPropID[] {
                        SevenZip.CoderPropID.LitContextBits,
                        SevenZip.CoderPropID.LitPosBits,
                        SevenZip.CoderPropID.PosStateBits,
                        SevenZip.CoderPropID.DictionarySize,
                        SevenZip.CoderPropID.EndMarker,
                        SevenZip.CoderPropID.MatchFinder,
                        SevenZip.CoderPropID.NumFastBytes,
                        SevenZip.CoderPropID.Algorithm
                    }, new object[] {
                        lc,
                        lp,
                        pb,
                        dictSize,
                        eos,
                        mf,
                        fb,
                        2
                    });
                    comp.SetTrainSize((uint)(train.Length - stream.skipSize));

                    output.WriteByte((byte)((pb * 5 + lp) * 9 + lc));
                    output.WriteByte((byte)dictLogSize);
                    output.WriteVLCI((ulong)dataLen);

                    comp.Code(stream, output, -1, -1, null);
                }
            }
        }

        public static void DecompressData(Stream input, byte[] training, Stream output)
        {
            byte[] reconsProp = new byte[5];
            int conf = input.ReadByte();
            int dictLogSize = input.ReadByte();
            if((conf | dictLogSize) < 0)
                throw new EndOfStreamException();

            if(dictLogSize > maxDictLogSize)
                throw new InvalidDataException();

            ulong len = input.ReadVLCI();

            reconsProp[0] = (byte)conf;
            uint dictSize = 1U << dictLogSize;
            for(int i = 0; i < 4; i++) {
                reconsProp[i+1] = (byte)(dictSize >> (i*8));
            }

            long compSize = input.Length - input.Position;

            SevenZip.Compression.LZMA.Decoder dec = new SevenZip.Compression.LZMA.Decoder();
            dec.SetDecoderProperties(reconsProp);
            if(training != null) {
                using(MemoryStream ms = new MemoryStream(training, false)) {
                    dec.Train(ms);
                }
            }

            dec.Code(input, output, compSize, (long)len, null);
        }

        //public static void Terminate(this object dummy) { }

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

        public static Models.ShipNameType LookupShipNameInfo(int id)
        {
            var ship = Grabacr07.KanColleWrapper.KanColleClient.Current.Master.Ships[id];
            return new Models.ShipNameType {
                ShipId = id,
                ShipName = ship?.Name ?? ("Ship" + id),
                TypeName = ship?.ShipType?.Name ?? ("Type"+id)
            };
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

        /*public static string LookupTypeName(int id)
        {
            var type = Grabacr07.KanColleWrapper.KanColleClient.Current.Master.ShipTypes[id];
            if(type == null) {
                return "Type" + id;
            }
            return type.Name;
        }*/

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

        public static IEnumerable<Tout> Zip<T1, T2, T3, Tout>(IEnumerable<T1> i1, IEnumerable<T2> i2, IEnumerable<T3> i3, Func<T1, T2, T3, Tout> project)
        {
            using (var e1 = i1.GetEnumerator())
            using (var e2 = i2.GetEnumerator())
            using (var e3 = i3.GetEnumerator()) {
                System.Collections.IEnumerator[] enumerators = new System.Collections.IEnumerator[] { e1, e2, e3 };
                while(enumerators.All(e => e.MoveNext())) {
                    yield return project(e1.Current, e2.Current, e3.Current);
                }
            }
        }

        public static IEnumerable<T> SafeConcat<T>(this IEnumerable<T> i1, params IEnumerable<T>[] i2)
        {
            if(i1 != null) {
                foreach(T v in i1) yield return v;
            }
            foreach(var i in i2) {
                if(i != null) {
                    foreach(T v in i) yield return v;
                }
            }
        }

        public static IEnumerable<Tout> SafeExpand<Tin, Tout>(this IEnumerable<Tin> i, Func<Tin, IEnumerable<Tout>> project)
        {
            if(i == null) yield break;
            foreach(var v in i) {
                var r = project(v);
                if(r == null) continue;
                foreach(var v1 in r) {
                    yield return v1;
                }
            }
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
    }
}
