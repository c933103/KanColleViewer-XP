using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Models
{
    [Serializable]
    public class Histogram<T> : IEnumerable<KeyValuePair<long, T>>
    {
        private readonly SortedDictionary<long, T> backend = new SortedDictionary<long, T>();

        public void Append(T val, long interval = -1)
        {
            if((backend.Count != 0) && backend.Last().Value.Equals(val)) {
                return;
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
        }

        public IEnumerator<KeyValuePair<long, T>> GetEnumerator()
        {
            return backend.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (backend as System.Collections.IEnumerable).GetEnumerator();
        }
    }
}
