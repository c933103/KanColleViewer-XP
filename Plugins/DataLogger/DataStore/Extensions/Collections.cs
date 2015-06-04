using LynLogger.DataStore.IO;
using LynLogger.DataStore.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.DataStore.Extensions
{
    static class Collections
    {
        public static Premitives.Dictionary<TPremitiveKey, TPremitiveValue> GetSerializationInfo<TKey, TValue, TPremitiveKey, TPremitiveValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dict, LinkedList<object> _path, Func<TKey, LinkedList<object>, TPremitiveKey> keyConverter, Func<TValue, LinkedList<object>, TPremitiveValue> valueConverter)
            where TPremitiveKey : Premitives.StoragePremitive, new()
            where TPremitiveValue : Premitives.StoragePremitive, new()
        {
            if (dict == null) return null;
            return new Premitives.Dictionary<TPremitiveKey, TPremitiveValue>(dict.Select(x => new KeyValuePair<TPremitiveKey, TPremitiveValue>(keyConverter(x.Key, _path), valueConverter(x.Value, _path))));
        }

        public static Premitives.Dictionary<TPremitiveKey, Premitives.Compound> GetSerializationInfo<TKey, TValue, TPremitiveKey>(this IEnumerable<KeyValuePair<TKey, TValue>> dict, LinkedList<object> _path, Func<TKey, LinkedList<object>, TPremitiveKey> keyConverter)
            where TPremitiveKey : Premitives.StoragePremitive, new()
            where TValue : AbstractDSSerializable<TValue>
        {
            if (dict == null) return null;
            return new Premitives.Dictionary<TPremitiveKey, Premitives.Compound>(dict.Select(x => new KeyValuePair<TPremitiveKey, Premitives.Compound>(keyConverter(x.Key, _path), (Premitives.Compound)x.Value.GetSerializationInfo(_path))));
        }

        public static Premitives.List<TPremitive> GetSerializationInfo<T, TPremitive>(this IEnumerable<T> list, LinkedList<object> _path, Func<T, LinkedList<object>, TPremitive> convert)
            where TPremitive : Premitives.StoragePremitive, new()
        {
            if (list == null) return null;
            return new Premitives.List<TPremitive>(list.Select(x => convert(x, _path)));
        }

        public static Premitives.List<Premitives.StoragePremitive> GetSerializationInfo<T>(this IEnumerable<T> list, LinkedList<object> _path)
            where T : IDSSerializable
        {
            if (list == null) return null;
            return new Premitives.List<Premitives.StoragePremitive>(list.Select(x => x.GetSerializationInfo(_path)));
        }

        public static IEnumerable<T> AsEnumerable<T>(params T[] ts) { return ts; }
    }
}
