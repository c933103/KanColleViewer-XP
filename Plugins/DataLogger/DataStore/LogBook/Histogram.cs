using LynLogger.DataStore.Serialization;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using LynLogger.DataStore.Premitives;
using LynLogger.DataStore.Extensions;
using System.Linq.Expressions;

namespace LynLogger.DataStore.LogBook
{
    public class Histogram<T> : IEnumerable<KeyValuePair<long, T>>, IDSSerializable
    {
        private readonly SortedDictionary<long, T> backend = new SortedDictionary<long, T>();
        private readonly ILogbook _holder;
        private static readonly Func<T, StoragePremitive> converter;
        private static readonly Func<StoragePremitive, LinkedList<object>, T> revConverter;

        static Histogram()
        {
            ParameterExpression inObj = Expression.Parameter(typeof(T));
            ParameterExpression inInfo = Expression.Parameter(typeof(StoragePremitive));
            ParameterExpression inPath = Expression.Parameter(typeof(LinkedList<object>));
            Type dataType = typeof(T);
            if(dataType.IsEnum) dataType = dataType.GetEnumUnderlyingType();
            converter = ExpressionGenerator<T>.Serializer;
            revConverter = ExpressionGenerator<T>.Deserializer;
        }

        public int Count { get { return backend.Count; } }

        public Histogram(ILogbook log) { _holder=log; }
        /*public Histogram(ILogbook log, IEnumerable<KeyValuePair<long, T>> data)
        {
            foreach(var ent in data) backend.Add(ent.Key, ent.Value);
        }*/

        public Histogram(StoragePremitive p, LinkedList<object> path)
        {
            path.AddFirst(this);
            foreach(var kv in ((Premitives.Dictionary<SignedInteger, StoragePremitive>)p).Convert((k, v) => new KeyValuePair<long, T>(k.Value, revConverter(v, path)))) {
                backend.Add(kv.Key, kv.Value);
            }
            path.RemoveFirst();
            _holder = (ILogbook)path.First(x => x is ILogbook);
        }

        public bool Append(T val, long interval = -1, bool keepDuplicate = true)
        {
            var ts = Helpers.UnixTimestamp;

            if(ts > _holder.EndTimestamp) return false;
            if(!keepDuplicate && (backend.Count != 0) && backend.Last().Value.Equals(val)) {
                return false;
            }
            if(interval < 0) {
                interval = 3600;
            }
            if((backend.Count >= 2) && (interval > 0)) {
                var last = backend.Skip(backend.Count - 2).ToArray();
                if(((ts - last[1].Key) < interval) && ((last[1].Key - last[0].Key) < interval)) {
                    backend.Remove(last[1].Key);
                } else if(val.Equals(last[1].Value) && last[1].Value.Equals(last[0].Value)) {
                    backend.Remove(last[1].Key);
                }
            }
            backend[ts] = val;
            return true;
        }

        public IEnumerator<KeyValuePair<long, T>> GetEnumerator()
        {
            return backend.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (backend as System.Collections.IEnumerable).GetEnumerator();
        }
        
        public StoragePremitive GetSerializationInfo()
        {
            var lastKey = backend.LastOrDefault().Key;
            return backend.SkipWhile(x => x.Key < _holder.StartTimestamp && x.Key != lastKey)
                          .TakeWhile(x => x.Key < _holder.EndTimestamp || x.Key == lastKey)
                          .GetSerializationInfo(x => new SignedInteger(x), converter);
        }
    }
}
