using LynLogger.DataStore.Extensions;
using LynLogger.DataStore.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LynLogger.DataStore.LogBook;
using System.Collections;
using LynLogger.Utilities;

namespace LynLogger.DataStore
{
    class Weekbook : AbstractDSSerializable<Weekbook>, ILogbook
    {
        [Serialize(0)] public BasicInfo BasicInfo { get; private set; }
        /*Serialize1*/ private SortedDictionary<int, Ship> _ships;
        public ulong SequenceId => 0;
        public string MemberId => _holder.MemberId;
        public long EndTimestamp => Helpers.UnixTimestamp;
        public long StartTimestamp => EndTimestamp - 7*86400;
        public IShipsLogAccessor Ships { get; }
        [Serialize(2)] private Histogram<SortieInfo> _sortieLog;
        [Serialize(3)] private Histogram<DrillInfo> _drillLog;
        [Serialize(6)] private Histogram<ShipCreate> _shipCreateLog;
        [Serialize(7)] private Histogram<ItemCreate> _itemCreateLog;

        public Histogram<ShipCreate> ShipCreateLog => _shipCreateLog ?? (_shipCreateLog = new Histogram<ShipCreate>(this));
        public Histogram<ItemCreate> ItemCreateLog => _itemCreateLog ?? (_itemCreateLog = new Histogram<ItemCreate>(this));
        public Histogram<SortieInfo> SortieLog => _sortieLog ?? (_sortieLog = new Histogram<SortieInfo>(this));
        public Histogram<DrillInfo> DrillLog => _drillLog ?? (_drillLog = new Histogram<DrillInfo>(this));

        private Store _holder;

        public Weekbook(Store s)
        {
            _holder = s;
            _ships = new SortedDictionary<int, Ship>();
            BasicInfo = new BasicInfo(this);
            Ships = new ShipsAccessor(this);
        }

        public Weekbook(Premitives.StoragePremitive info, LinkedList<object> serializationPath) : base(info, serializationPath)
        {
            _holder = (Store)serializationPath.First(x => x is Store);
            Ships = new ShipsAccessor(this);

            var keys = _ships.Where(kv => !(kv.Value?.ShipNameType?.Count > 0) || !(kv.Value?.UpdateTime > StartTimestamp)).Select(x => x.Key).ToList();
            foreach(var k in keys) _ships.Remove(k);
        }

        protected override IDictionary<ulong, HandlerInfo> CustomFieldHandlers
        {
            get
            {
                return new Dictionary<ulong, HandlerInfo>() {
                    [1] = new HandlerInfo(
                        (x, p) => x._ships.GetSerializationInfo(p, (k, p1) => new Premitives.SignedInteger(k)),
                        (o, i, p) => o._ships = new SortedDictionary<int, Ship>(
                                                    (i as Premitives.DsDictionary<Premitives.SignedInteger, Premitives.StoragePremitive>)?.Convert(x => (int)x.Value, x => new Ship(x, p))
                                                 ?? (i as Premitives.DsDictionary<Premitives.SignedInteger, Premitives.Compound>)?.Convert(x => (int)x.Value, x => new Ship(x, p)))
                    ),
                };
            }
        }

        public class ShipsAccessor : IShipsLogAccessor
        {
            private Weekbook lbk;
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

            public ShipsAccessor(Weekbook lbk) { this.lbk = lbk; }
            public bool Contains(int id) => lbk._ships.ContainsKey(id);
            public bool Remove(int id) => lbk._ships.Remove(id);

            public IEnumerator<Ship> GetEnumerator() { return lbk._ships.Values.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }
    }
}
