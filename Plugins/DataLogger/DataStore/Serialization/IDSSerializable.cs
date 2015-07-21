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
        private static ConcurrentDictionary<ulong, HandlerInfo> _serializationHandlers = null;

        protected virtual ulong StructureVersion => 0;
        protected ulong DeserializedStructureVersion { get; private set; }

        protected virtual IReadOnlyDictionary<ulong, HandlerInfo> CustomFieldHandlers => null;

        private IDictionary<ulong, Premitives.StoragePremitive> _blackBox;
        protected IDictionary<ulong, Premitives.StoragePremitive> BlackBox => _blackBox ?? (_blackBox = new Dictionary<ulong, Premitives.StoragePremitive>());

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
                    BlackBox.Add(kv.Key, kv.Value);
                } else {
                    if (depth > handler.DepthLimit) continue;
                    if (handler.IgnoreIfNull && kv.Value == null) continue;
                    handler.Deserialize((T)this, kv.Value, serializationPath);
                }
            }
            serializationPath.RemoveFirst();
        }

        public virtual Premitives.StoragePremitive GetSerializationInfo(LinkedList<object> serializationPath)
        {
            InitHandlers();

            var r = new Premitives.Compound();
            foreach(var kv in BlackBox) {
                r[kv.Key] = kv.Value;
            }

            if (serializationPath == null) serializationPath = new LinkedList<object>();
            serializationPath.AddFirst(this);
            var depth = serializationPath.Count;
            foreach (var kv in _serializationHandlers) {
                if (depth > kv.Value.DepthLimit) continue;
                var si = kv.Value.Serialize((T)this, serializationPath);
                if (kv.Value.IgnoreIfNull && si == null) continue;
                r[kv.Key] = si;
            }
            serializationPath.RemoveFirst();

            return r;
        }

        private void InitHandlers()
        {
            if(_serializationHandlers != null) return;

            var localHandlers = new ConcurrentDictionary<ulong, HandlerInfo> {
                [0] = new HandlerInfo((x, p) => x.StructureVersion != 0 ? new Premitives.UnsignedInteger(x.StructureVersion) : null, (o, i, p) => o.DeserializedStructureVersion = ((Premitives.UnsignedInteger)i)?.Value ?? 0)
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
            public readonly bool IgnoreIfNull;

            public HandlerInfo(Func<T, LinkedList<object>, Premitives.StoragePremitive> s, Action<T, Premitives.StoragePremitive, LinkedList<object>> d, bool ignoreNull = false) : this(int.MaxValue, s, d, ignoreNull) { }

            public HandlerInfo(int l, Func<T, LinkedList<object>, Premitives.StoragePremitive> s, Action<T, Premitives.StoragePremitive, LinkedList<object>> d, bool ignoreNull = false)
            {
                DepthLimit = l; Serialize = s; Deserialize = d; IgnoreIfNull = ignoreNull;
            }

            public static readonly HandlerInfo NoOp = new HandlerInfo(0, null, null, true);
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
