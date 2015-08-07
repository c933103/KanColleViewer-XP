using LynLogger.DataStore.Premitives;
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
        public static DsDictionary<TPremitiveKey, TPremitiveValue> GetSerializationInfo<TKey, TValue, TPremitiveKey, TPremitiveValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dict, LinkedList<object> _path, Func<TKey, LinkedList<object>, TPremitiveKey> keyConverter, Func<TValue, LinkedList<object>, TPremitiveValue> valueConverter)
            where TPremitiveKey : StoragePremitive, new()
            where TPremitiveValue : StoragePremitive, new()
        {
            if (dict == null) return null;
            return new DsDictionary<TPremitiveKey, TPremitiveValue>(dict.Select(x => new KeyValuePair<TPremitiveKey, TPremitiveValue>(keyConverter(x.Key, _path), valueConverter(x.Value, _path))));
        }

        public static Premitives.DsDictionary<Premitives.StoragePremitive, Premitives.StoragePremitive> GetSerializationInfo<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dict, LinkedList<object> _path)
            where TKey : IDSSerializable
            where TValue : IDSSerializable
        {
            if (dict == null) return null;
            return new Premitives.DsDictionary<Premitives.StoragePremitive, Premitives.StoragePremitive>(dict.Select(x => new KeyValuePair<StoragePremitive, StoragePremitive>(x.Key.GetSerializationInfo(_path), x.Value.GetSerializationInfo(_path))));
        }

        public static DsDictionary<TPremitiveKey, StoragePremitive> GetSerializationInfo<TKey, TValue, TPremitiveKey>(this IEnumerable<KeyValuePair<TKey, TValue>> dict, LinkedList<object> _path, Func<TKey, LinkedList<object>, TPremitiveKey> keyConverter)
            where TPremitiveKey : StoragePremitive, new()
            where TValue : IDSSerializable
        {
            if (dict == null) return null;
            return new DsDictionary<TPremitiveKey, StoragePremitive>(dict.Select(x => new KeyValuePair<TPremitiveKey, StoragePremitive>(keyConverter(x.Key, _path), x.Value.GetSerializationInfo(_path))));
        }

        public static DsList<TPremitive> GetSerializationInfo<T, TPremitive>(this IEnumerable<T> list, LinkedList<object> _path, Func<T, LinkedList<object>, TPremitive> convert)
            where TPremitive : StoragePremitive, new()
        {
            if (list == null) return null;
            return new DsList<TPremitive>(list.Select(x => convert(x, _path)));
        }

        public static DsList<StoragePremitive> GetSerializationInfo<T>(this IEnumerable<T> list, LinkedList<object> _path)
            where T : IDSSerializable
        {
            if (list == null) return null;
            return new DsList<StoragePremitive>(list.Select(x => x.GetSerializationInfo(_path)));
        }
    }
}
