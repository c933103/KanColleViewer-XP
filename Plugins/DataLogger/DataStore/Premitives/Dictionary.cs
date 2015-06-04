using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LynLogger.DataStore.IO;
using LynLogger.DataStore.Extensions;

namespace LynLogger.DataStore.Premitives
{
    [Serializable]
    class Dictionary<TKey, TValue> : StoragePremitive
        where TKey : StoragePremitive, new()
        where TValue : StoragePremitive, new()
    {
        private IList<KeyValuePair<TKey, TValue>> data = new System.Collections.Generic.List<KeyValuePair<TKey, TValue>>();

        public override IEnumerable<TypeIdentifier> Type => Collections.AsEnumerable(TypeIdentifier.Dictionary).Concat(new TKey().Type).Concat(new TValue().Type);
        public int Count => data.Count;

        public Dictionary() { }
        public Dictionary(IEnumerable<KeyValuePair<TKey, TValue>> s) { foreach(var kv in s) data.Add(kv); }
        public Dictionary(DSReader input)
        {
            var fcount = input.Read7bUInt();
            for(uint i = 0; i < fcount; i++) {
                var k = Parse(input);
                var v = Parse(input);
                data.Add(new KeyValuePair<TKey, TValue>((TKey)k, (TValue)v));
            }
        }

        public override void SerializeNonNull(DSWriter output)
        {
            output.Write(Type);
            output.Write7bUInt((uint)data.Count);
            foreach(var kv in data) {
                kv.Key.Serialize(output);
                kv.Value.Serialize(output);
            }
        }

        public System.Collections.Generic.Dictionary<TOutputKey, TOutputValue> Convert<TOutputKey, TOutputValue>(Func<TKey, TOutputKey> keyConverter, Func<TValue, TOutputValue> valueConverter)
        {
            return data.ToDictionary(x => keyConverter(x.Key), x => valueConverter(x.Value));
        }

        public IEnumerable<TOutput> Convert<TOutput>(Func<TKey, TValue, TOutput> converter)
        {
            return data.Select(x => converter(x.Key, x.Value));
        }
    }
}
