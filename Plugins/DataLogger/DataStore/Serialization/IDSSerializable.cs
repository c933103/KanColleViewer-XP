using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace LynLogger.DataStore.Serialization
{
    public interface IDSSerializable
    {
        Premitives.StoragePremitive GetSerializationInfo();
    }

    [Serializable]
    public abstract partial class AbstractDSSerializable<T> : IDSSerializable
        where T : AbstractDSSerializable<T>
    {
        protected virtual ulong StructureVersion { get { return 0; } }
        protected ulong DeserializedStructureVersion { get; private set; }

        protected virtual IDictionary<ulong, HandlerInfo> CustomFieldHandlers { get { return null; } }

        private static ConcurrentDictionary<ulong, HandlerInfo> _serializationHandlers = null;
        private IDictionary<ulong, Premitives.StoragePremitive> blackBox = new Dictionary<ulong, Premitives.StoragePremitive>();

        protected AbstractDSSerializable() { DeserializedStructureVersion = StructureVersion; }

        protected AbstractDSSerializable(Premitives.StoragePremitive storage, LinkedList<object> serializationPath)
        {
            InitHandlers();

            serializationPath.AddFirst(this);
            var info = (Premitives.Compound)storage;
            foreach(var kv in info) {
                if(!_serializationHandlers.ContainsKey(kv.Key)) {
                    blackBox.Add(kv.Key, kv.Value);
                } else {
                    _serializationHandlers[kv.Key].Deserialize((T)this, kv.Value, serializationPath);
                }
            }
            serializationPath.RemoveFirst();
        }

        public virtual Premitives.StoragePremitive GetSerializationInfo()
        {
            InitHandlers();

            var r = new Premitives.Compound();
            foreach(var kv in blackBox) {
                r[kv.Key] = kv.Value;
            }
            foreach(var kv in _serializationHandlers) {
                r[kv.Key] = kv.Value.Serialize((T)this);
            }
            return r;
        }

        private void InitHandlers()
        {
            if(_serializationHandlers != null) return;

            var localHandlers = new ConcurrentDictionary<ulong, HandlerInfo> {
                [0] = new HandlerInfo(x => new Premitives.UnsignedInteger(x.StructureVersion), (o, i, p) => o.DeserializedStructureVersion = ((Premitives.UnsignedInteger)i).Value)
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
            public Func<T, Premitives.StoragePremitive> Serialize;
            public Action<T, Premitives.StoragePremitive, LinkedList<object>> Deserialize;

            public HandlerInfo(Func<T, Premitives.StoragePremitive> s, Action<T, Premitives.StoragePremitive, LinkedList<object>> d)
            {
                Serialize = s; Deserialize = d;
            }
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

        public ulong FieldId
        {
            get { return fieldId; }
        }

        public Type ConstructionType { get; set; }
    }
}
