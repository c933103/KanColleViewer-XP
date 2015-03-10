using System.Collections.Generic;
using System.Linq;

namespace LynLogger.Models.Merge
{
    static class DictionaryHelper
    {
        public static void Merge<K, V>(this IDictionary<K, V> dictTo, IDictionary<K, V> dictFrom)
        {
            foreach(var kv in dictFrom) {
                if(dictTo.ContainsKey(kv.Key)) {
                    if(typeof(V).FindInterfaces((type, _) => type == typeof(IMergable<V>), null).Any()) {
                        (dictTo[kv.Key] as IMergable<V>).Merge(dictFrom[kv.Key]);
                    }
                } else {
                    dictTo[kv.Key] = dictFrom[kv.Key];
                }
            }
        }
    }

    interface IMergable<T> { void Merge(T val); }
}
