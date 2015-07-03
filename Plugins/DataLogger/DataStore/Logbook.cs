using System;
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
        [Serialize(4)] private Histogram<SortieInfo> _sortieLog;
        [Serialize(5)] private Histogram<DrillInfo> _drillLog;
        [Serialize(6)] private Histogram<ShipCreate> _shipCreateLog;
        [Serialize(7)] private Histogram<ItemCreate> _itemCreateLog;

        public Histogram<ShipCreate> ShipCreateLog => _shipCreateLog ?? (_shipCreateLog = new Histogram<ShipCreate>(this));
        public Histogram<ItemCreate> ItemCreateLog => _itemCreateLog ?? (_itemCreateLog = new Histogram<ItemCreate>(this));
        public Histogram<SortieInfo> SortieLog => _sortieLog ?? (_sortieLog = new Histogram<SortieInfo>(this));
        public Histogram<DrillInfo> DrillLog => _drillLog ?? (_drillLog = new Histogram<DrillInfo>(this));
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
                        (x, p) => x._ships.GetSerializationInfo(p, (k, p1) => new Premitives.SignedInteger(k)),
                        (o, i, p) => o._ships = new SortedDictionary<int, Ship>(
                                                    (i as Premitives.Dictionary<Premitives.SignedInteger, Premitives.StoragePremitive>)?.Convert(x => (int)x.Value, x => new Ship(x, p))
                                                 ?? (i as Premitives.Dictionary<Premitives.SignedInteger, Premitives.Compound>)?.Convert(x => (int)x.Value, x => new Ship(x, p)))
                    ),
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
        Histogram<SortieInfo> SortieLog { get; }
        Histogram<DrillInfo> DrillLog { get; }
        Histogram<ShipCreate> ShipCreateLog { get; }
        Histogram<ItemCreate> ItemCreateLog { get; }
    }

    public interface IShipsLogAccessor : IEnumerable<Ship>
    {
        Ship this[int id] { get; }
        bool Contains(int id);
    }
}
