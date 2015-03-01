﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Models.Scavenge
{
    static class DictionaryHelper
    {
        public static int Scavenge<K, V>(this IDictionary<K, V> dict, IScavenger sc, KeyValuePair<Type, Type>[] targetTypes, bool keepOne)
        {
            var thisType = new KeyValuePair<Type, Type>(typeof(K), typeof(V));
            var scavengeCount = 0;
            if(targetTypes.Any(kv => kv.Key == thisType.Key && kv.Value == thisType.Value)) {
                sc.Reset();
                if(sc is IRampUpScavenger) {
                    var rusc = sc as IRampUpScavenger;
                    foreach(var kv in dict) {
                        rusc.RampUp(kv.Key, kv.Value);
                    }
                }
                var removeKeys = dict.Where(kv => !sc.ShouldKeep(kv.Key, kv.Value)).Select(kv => kv.Key).ToList();
                if(keepOne && removeKeys.Count > 0) removeKeys.RemoveAt(removeKeys.Count-1);
                foreach(var key in removeKeys) {
                    dict.Remove(key);
                }
                scavengeCount += removeKeys.Count;
            }
            if(typeof(V).FindInterfaces((type, _) => type == typeof(IScavengable), null).Any()) {
                foreach(var scavengable in dict.Values.Cast<IScavengable>()) {
                    scavengeCount += scavengable.Scavenge(sc, targetTypes);
                }
            }
            return scavengeCount;
        }
    }
}
