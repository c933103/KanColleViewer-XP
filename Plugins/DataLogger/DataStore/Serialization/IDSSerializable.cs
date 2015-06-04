using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace LynLogger.DataStore.Serialization
{
    public interface IDSSerializable
    {
        Premitives.StoragePremitive GetSerializationInfo(LinkedList<object> _path);
    }

    [Serializable]
    public abstract partial class AbstractDSSerializable<T> : IDSSerializable
        where T : AbstractDSSerializable<T>
    {
        protected virtual ulong StructureVersion => 0;
        protected ulong DeserializedStructureVersion { get; private set; }

        protected virtual IDictionary<ulong, HandlerInfo> CustomFieldHandlers => null;

        private static ConcurrentDictionary<ulong, HandlerInfo> _serializationHandlers = null;
        private IDictionary<ulong, Premitives.StoragePremitive> blackBox = new Dictionary<ulong, Premitives.StoragePremitive>();

        protected AbstractDSSerializable() { DeserializedStructureVersion = StructureVersion; }

        protected AbstractDSSerializable(Premitives.StoragePremitive storage, LinkedList<object> serializationPath)
        {
            InitHandlers();

            serializationPath.AddFirst(this);
            var info = (Premitives.Compound)storage;
            var depth = serializationPath.Count;
            foreach (var kv in info) {
                HandlerInfo handler;
                if(!_serializationHandlers.TryGetValue(kv.Key, out handler)) {
                    blackBox.Add(kv.Key, kv.Value);
                } else {
                    if (depth > handler.DepthLimit) continue;
                    handler.Deserialize((T)this, kv.Value, serializationPath);
                }
            }
            serializationPath.RemoveFirst();
        }

        public virtual Premitives.StoragePremitive GetSerializationInfo(LinkedList<object> serializationPath)
        {
            InitHandlers();

            var r = new Premitives.Compound();
            foreach(var kv in blackBox) {
                r[kv.Key] = kv.Value;
            }

            if (serializationPath == null) serializationPath = new LinkedList<object>();
            serializationPath.AddFirst(this);
            var depth = serializationPath.Count;
            foreach (var kv in _serializationHandlers) {
                if (depth > kv.Value.DepthLimit) continue;
                r[kv.Key] = kv.Value.Serialize((T)this, serializationPath);
            }
            serializationPath.RemoveFirst();

            return r;
        }

        private void InitHandlers()
        {
            if(_serializationHandlers != null) return;

            var localHandlers = new ConcurrentDictionary<ulong, HandlerInfo> {
                [0] = new HandlerInfo((x, p) => new Premitives.UnsignedInteger(x.StructureVersion), (o, i, p) => o.DeserializedStructureVersion = ((Premitives.UnsignedInteger)i).Value)
            };
            foreach(var kv in DiscoverFields()) {
                localHandlers[kv.Key+16] = kv.Value;
            }
            if(CustomFieldHandlers != null) {
                foreach(var kv in CustomFieldHandlers) {
                    localHandlers[kv.Key+16] = kv.Value;
                }
            }
            Interlocked.CompareExchange(ref _serializationHandlers, localHandlers, null);
        }

        protected class HandlerInfo
        {
            public readonly Func<T, LinkedList<object>, Premitives.StoragePremitive> Serialize;
            public readonly Action<T, Premitives.StoragePremitive, LinkedList<object>> Deserialize;
            public readonly int DepthLimit;

            public HandlerInfo(Func<T, LinkedList<object>, Premitives.StoragePremitive> s, Action<T, Premitives.StoragePremitive, LinkedList<object>> d) : this(int.MaxValue, s, d) { }

            public HandlerInfo(int l, Func<T, LinkedList<object>, Premitives.StoragePremitive> s, Action<T, Premitives.StoragePremitive, LinkedList<object>> d)
            {
                DepthLimit = l; Serialize = s; Deserialize = d;
            }

            public static readonly HandlerInfo NoOp = new HandlerInfo(0, null, null);
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    sealed class SerializeAttribute : Attribute
    {
        readonly ulong fieldId;

        public SerializeAttribute(ulong fieldId)
        {
            this.fieldId = fieldId;
        }

        public ulong FieldId => fieldId;

        public Type ConstructionType { get; set; }
        public int DepthLimit { get; set; } = int.MaxValue;
    }
}
