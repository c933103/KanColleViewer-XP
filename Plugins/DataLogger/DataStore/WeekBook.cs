using LynLogger.DataStore.Extensions;
using LynLogger.DataStore.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LynLogger.DataStore.LogBook;
using System.Collections;

namespace LynLogger.DataStore
{
    class Weekbook : AbstractDSSerializable<Weekbook>, ILogbook
    {
        [Serialize(0)] public BasicInfo BasicInfo { get; private set; }
        /*Serialize1*/ private SortedDictionary<int, Ship> _ships;
        public ulong SequenceId { get { return 0; } }
        public string MemberId { get { return _holder.MemberId; } }
        public long EndTimestamp { get { return Helpers.UnixTimestamp; } }
        public long StartTimestamp { get { return EndTimestamp - 7*86400; } }
        public IShipsLogAccessor Ships { get; }

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
                        x => x._ships.GetSerializationInfo(k => new Premitives.SignedInteger(k)),
                        (o, i, p) => o._ships = new SortedDictionary<int, Ship>(((Premitives.Dictionary<Premitives.SignedInteger, Premitives.Compound>)i).Convert(x => (int)x.Value, x => new Ship(x, p)))),
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
            public bool Contains(int id) { return lbk._ships.ContainsKey(id); }
            public IEnumerator<Ship> GetEnumerator() { return lbk._ships.Values.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }
    }
}
