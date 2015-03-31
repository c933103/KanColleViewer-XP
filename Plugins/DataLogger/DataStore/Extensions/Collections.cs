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
        public static Premitives.Dictionary<TPremitiveKey, TPremitiveValue> GetSerializationInfo<TKey, TValue, TPremitiveKey, TPremitiveValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dict, Func<TKey, TPremitiveKey> keyConverter, Func<TValue, TPremitiveValue> valueConverter)
            where TPremitiveKey : Premitives.StoragePremitive, new()
            where TPremitiveValue : Premitives.StoragePremitive, new()
        {
            return new Premitives.Dictionary<TPremitiveKey, TPremitiveValue>(dict.Select(x => new KeyValuePair<TPremitiveKey, TPremitiveValue>(keyConverter(x.Key), valueConverter(x.Value))));
        }

        /*public static Premitives.Dictionary<TPremitiveKey, Premitives.StoragePremitive> GetSerializationInfo<TKey, TValue, TPremitiveKey>(this IEnumerable<KeyValuePair<TKey, TValue>> dict, Func<TKey, TPremitiveKey> keyConverter)
            where TPremitiveKey : Premitives.StoragePremitive, new()
            where TValue : IDSSerializable
        {
            return new Premitives.Dictionary<TPremitiveKey, Premitives.StoragePremitive>(dict.Select(x => new KeyValuePair<TPremitiveKey, Premitives.StoragePremitive>(keyConverter(x.Key), x.Value.GetSerializationInfo())));
        }*/

        public static Premitives.Dictionary<TPremitiveKey, Premitives.Compound> GetSerializationInfo<TKey, TValue, TPremitiveKey>(this IEnumerable<KeyValuePair<TKey, TValue>> dict, Func<TKey, TPremitiveKey> keyConverter)
            where TPremitiveKey : Premitives.StoragePremitive, new()
            where TValue : AbstractDSSerializable<TValue>
        {
            return new Premitives.Dictionary<TPremitiveKey, Premitives.Compound>(dict.Select(x => new KeyValuePair<TPremitiveKey, Premitives.Compound>(keyConverter(x.Key), (Premitives.Compound)x.Value.GetSerializationInfo())));
        }

        public static Premitives.List<TPremitive> GetSerializationInfo<T, TPremitive>(this IEnumerable<T> list, Func<T, TPremitive> convert)
            where TPremitive : Premitives.StoragePremitive, new()
        {
            return new Premitives.List<TPremitive>(list.Select(x => convert(x)));
        }

        public static IEnumerable<T> AsEnumerable<T>(params T[] ts) { return ts; }
    }
}
