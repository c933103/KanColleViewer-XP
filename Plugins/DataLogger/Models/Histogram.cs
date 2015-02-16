using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Models
{
    [Serializable]
    public class Histogram<T> : IEnumerable<KeyValuePair<long, Histogram<T>.Record>>
    {
        private readonly SortedDictionary<long, Record> backend = new SortedDictionary<long, Record>();

        public T this[long seq]
        {
            get
            {
                if(backend.ContainsKey(seq)) {
                    return backend[seq].Value;
                }
                return default(T);
            }
            set
            {
                if(backend.LastOrDefault().Value.Value.Equals(value)) return;
                backend[seq] = new Record(Helpers.UnixTimestamp, value);
            }
        }

        public void Append(T val)
        {
            this[backend.Keys.LastOrDefault()+1] = val;
        }

        public IEnumerator<KeyValuePair<long, Record>> GetEnumerator()
        {
            foreach(var rec in backend) {
                yield return new KeyValuePair<long, Record>(rec.Key, rec.Value);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach(var rec in backend) {
                yield return new KeyValuePair<long, Record>(rec.Key, rec.Value);
            }
        }

        [Serializable]
        public struct Record
        {
            public readonly long Timestamp;
            public readonly T Value;

            public Record(long ts, T val)
            {
                Timestamp = ts; Value = val;
            }
        }
    }
}
