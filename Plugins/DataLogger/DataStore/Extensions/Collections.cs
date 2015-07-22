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

        public static Premitives.Dictionary<Premitives.StoragePremitive, Premitives.StoragePremitive> GetSerializationInfo<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dict, LinkedList<object> _path)
            where TKey : IDSSerializable
            where TValue : IDSSerializable
        {
            if (dict == null) return null;
            return new Premitives.Dictionary<Premitives.StoragePremitive, Premitives.StoragePremitive>(dict.Select(x => new KeyValuePair<Premitives.StoragePremitive, Premitives.StoragePremitive>(x.Key.GetSerializationInfo(_path), x.Value.GetSerializationInfo(_path))));
        }

        public static Premitives.Dictionary<TPremitiveKey, Premitives.StoragePremitive> GetSerializationInfo<TKey, TValue, TPremitiveKey>(this IEnumerable<KeyValuePair<TKey, TValue>> dict, LinkedList<object> _path, Func<TKey, LinkedList<object>, TPremitiveKey> keyConverter)
            where TPremitiveKey : Premitives.StoragePremitive, new()
            where TValue : IDSSerializable
        {
            if (dict == null) return null;
            return new Premitives.Dictionary<TPremitiveKey, Premitives.StoragePremitive>(dict.Select(x => new KeyValuePair<TPremitiveKey, Premitives.StoragePremitive>(keyConverter(x.Key, _path), x.Value.GetSerializationInfo(_path))));
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
    }
}
