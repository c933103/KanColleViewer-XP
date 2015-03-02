using LynLogger.Models.Merge;
using LynLogger.Models.Scavenge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Models
{
    [Serializable]
    public class Histogram<T> : IEnumerable<KeyValuePair<long, T>>, IScavengable, IMergable<Histogram<T>>
    {
        private readonly SortedDictionary<long, T> backend = new SortedDictionary<long, T>();

        public int Count { get { return backend.Count; } }

        public Histogram() { }
        internal Histogram(IEnumerable<KeyValuePair<long, T>> data)
        {
            foreach(var ent in data) backend.Add(ent.Key, ent.Value);
        }

        public bool Append(T val, long interval = -1, bool keepDuplicate = false)
        {
            if(!keepDuplicate && (backend.Count != 0) && backend.Last().Value.Equals(val)) {
                return false;
            }
            if(interval < 0) {
                interval = 3600;
            }
            if((backend.Count >= 2) && (interval > 0)) {
                var last = backend.Skip(backend.Count - 2).ToArray();
                if(((Helpers.UnixTimestamp - last[1].Key) < interval)
                && ((last[1].Key - last[0].Key) < interval)) {
                    backend.Remove(last[1].Key);
                }
            }
            backend[Helpers.UnixTimestamp] = val;
            return true;
        }

        public int RemoveBefore(long ts)
        {
            var deleteKeys = backend.Select(kv => kv.Key).TakeWhile(t => t < ts).ToList();
            foreach(var t in deleteKeys) {
                backend.Remove(t);
            }
            return deleteKeys.Count;
        }

        public IEnumerator<KeyValuePair<long, T>> GetEnumerator()
        {
            return backend.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (backend as System.Collections.IEnumerable).GetEnumerator();
        }

        public int Scavenge(IScavenger sc, KeyValuePair<Type, Type>[] targetTypes)
        {
            return backend.Scavenge(sc, targetTypes, true);
        }

        public void Merge(Histogram<T> val)
        {
            backend.Merge(val.backend);
        }
    }
}
