﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LynLogger.DataStore.LogBook;
using LynLogger.DataStore.Extensions;
using LynLogger.DataStore.Serialization;
using System.Collections;

namespace LynLogger.DataStore
{
    class Logbook : AbstractDSSerializable<Logbook>, ILogbook
    {
        [Serialize(0)] public string MemberId { get; private set; }
        [Serialize(1)] public ulong SequenceId { get; private set; }
        [Serialize(2)] public BasicInfo BasicInfo { get; private set; }
        /*Serialize3*/ private SortedDictionary<int, Ship> _ships;
        public long StartTimestamp { get; }
        public long EndTimestamp { get; }

        public IShipsLogAccessor Ships { get; }

        public Logbook(string memId, ulong seqId)
        {
            MemberId = memId;
            SequenceId = seqId;
            BasicInfo = new BasicInfo(this);
            _ships = new SortedDictionary<int, Ship>();

            Ships = new ShipsAccessor(this);
            StartTimestamp = Helpers.FromLogbookSequence(SequenceId).ToUnixTimestamp();
            EndTimestamp = Helpers.FromLogbookSequence(SequenceId+1).ToUnixTimestamp();
        }

        public Logbook(Premitives.StoragePremitive info, LinkedList<object> serializationPath) : base(info, serializationPath)
        {
            Ships = new ShipsAccessor(this);
            StartTimestamp = Helpers.FromLogbookSequence(SequenceId).ToUnixTimestamp();
            EndTimestamp = Helpers.FromLogbookSequence(SequenceId+1).ToUnixTimestamp();

            var keys = _ships.Where(kv => !(kv.Value?.ShipNameType?.Count > 0) || !(kv.Value?.UpdateTime > StartTimestamp) ).Select(x => x.Key).ToList();
            foreach(var k in keys) _ships.Remove(k);
        }

        protected override IDictionary<ulong, HandlerInfo> CustomFieldHandlers
        {
            get
            {
                return new Dictionary<ulong, HandlerInfo>() {
                    [3] = new HandlerInfo(
                        x => x._ships.GetSerializationInfo(k => new Premitives.SignedInteger(k)),
                        (o, i, p) => o._ships = new SortedDictionary<int, Ship>(((Premitives.Dictionary<Premitives.SignedInteger, Premitives.Compound>)i).Convert(x => (int)x.Value, x => new Ship(x, p)))),
                };
            }
        }
            

        public class ShipsAccessor : IShipsLogAccessor
        {
            private Logbook lbk;
            public Ship this[int id]
            {
                get
                {
                    if(lbk._ships.ContainsKey(id)) return lbk._ships[id];

                    Ship s = new Ship(lbk, id);
                    lbk._ships.Add(id, s);
                    return s;
                }
            }

            public ShipsAccessor(Logbook lbk) { this.lbk = lbk; }
            public bool Contains(int id) { return lbk._ships.ContainsKey(id); }
            public IEnumerator<Ship> GetEnumerator() { return lbk._ships.Values.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }
    }

    public interface ILogbook
    {
        string MemberId { get; }
        ulong SequenceId { get; }
        BasicInfo BasicInfo { get; }
        IShipsLogAccessor Ships { get; }
        long StartTimestamp { get; }
        long EndTimestamp { get; }
    }

    public interface IShipsLogAccessor : IEnumerable<Ship>
    {
        Ship this[int id] { get; }
        bool Contains(int id);
    }
}