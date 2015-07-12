using LynLogger.DataStore.IO;
using LynLogger.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LynLogger.DataStore.Extensions;
using LynLogger.DataStore.Serialization;
using LynLogger.DataStore.MasterInfo;

namespace LynLogger.DataStore
{
    public class Store : AbstractDSSerializable<Store>
    {
        private static Action<Store> _onDataStoreCreate;
        private static Action<Store> _onDataStoreSwitch;

        public static event Action<Store> OnDataStoreCreate {
            add
            {
                var dss = _ds.ToList();
                foreach (var ds in dss) {
                    Store s;
                    if (ds.Value.TryGetTarget(out s)) {
                        value(s);
                    } else {
                        _ds.Remove(ds.Key);
                    }
                }
                _onDataStoreCreate += value.MakeWeak(x => _onDataStoreCreate -= x);
            }
            remove { }
        }

        public static event Action<Store> OnDataStoreSwitch {
            add
            {
                if (Current != null) value(Current);
                _onDataStoreSwitch += value.MakeWeak(x => _onDataStoreSwitch -= x);
            }
            remove { }
        }

        private Action<Store> _onBasicInfoChange;
        private Action<Store, int> _onShipDataChange;
        public event Action<Store> OnBasicInfoChange { add { _onBasicInfoChange += value.MakeWeak(x => _onBasicInfoChange -= x); } remove { } }
        public event Action<Store, int> OnShipDataChange { add { _onShipDataChange += value.MakeWeak(x => _onShipDataChange -= x); } remove { } }

        public static Store Current { get; private set; }

        private static readonly Dictionary<string, WeakReference<Store>> _ds = new Dictionary<string, WeakReference<Store>>();
        private static readonly string _dataDir = Path.Combine(Environment.CurrentDirectory, "LynLogger");
        private static byte[] _logHeader = new byte[] { 0x48, 0x41, 0x49, 0x49, 0x4C, 0x4F, 0x47, 0x00 };
        private static byte[] _masterHeader = new byte[] { 0x48, 0x41, 0x49, 0x49, 0x4D, 0x53, 0x54, 0x00 };

        private static bool _evRegistered = false;

        internal static void Refresh()
        {
            if(_onDataStoreSwitch != null) _onDataStoreSwitch(Current);
        }

        internal static void SwitchMember(string memberId)
        {
            if(Current?.MemberId == memberId) return;
            Current?.SaveData();

            if(!_evRegistered) {
                _evRegistered = true;
                System.Windows.Application.Current.Exit += (s, e) => Current?.SaveData();
            }

            Store store;
            if (!_ds.ContainsKey(memberId) || !_ds[memberId].TryGetTarget(out store)) {
                var masterTable = Path.Combine(_dataDir, MasterTableFilename(memberId));
                if(File.Exists(masterTable)) {
                    try {
                        using (Stream input = File.OpenRead(masterTable)) {
                            byte[] buf = new byte[8];
                            input.ReadFully(buf);
                            if(buf.SequenceEqual(_masterHeader)) {
                                //deserialize
                                using (DSReader rdr = new DSReader(input, true)) {
                                    store = new Store(Premitives.StoragePremitive.Parse(rdr), new LinkedList<object>());
                                    goto register;
                                }
                            }
                        }
                    } catch (IOException) { }
                }
                store = new Store(memberId);

                register:
                if (_onDataStoreCreate != null) _onDataStoreCreate(store);
                _ds.Add(memberId, new WeakReference<Store>(store));
            }

            Current = store;
            if (_onDataStoreSwitch != null) _onDataStoreSwitch(Current);
        }

        private static Logbook LoadLogbook(Store s, ulong seq)
        {
            // The record isn't authoritive. Don't bother checking.
            // if(!s.MasterInfo.LogbookSequence.Contains(seq)) return new Logbook(s.MasterInfo.MemberId, seq);
            var file = Path.Combine(_dataDir, LogbookFilename(s.MemberId, seq));
            if(File.Exists(file)) {
                try {
                    using (Stream input = File.OpenRead(file)) {
                        //check header
                        byte[] buf = new byte[8];
                        input.ReadFully(buf);
                        if(buf.SequenceEqual(_logHeader)) {
                            //deserialize
                            using (DSReader rdr = new DSReader(input, true)) {
                                return new Logbook(Premitives.StoragePremitive.Parse(rdr), new LinkedList<object>());
                            }
                        }

                    }
                } catch (IOException) {
                    //Fall through.
                }
            }
            var newBook = new Logbook(s.MemberId, seq);
            s._dirtyLogbooks.Add(newBook);
            return newBook;
        }

        private static string MasterTableFilename(string memberId)
        {
            return string.Format("{0}.master", Encoding.UTF8.GetBytes(memberId).Base32Encode());
        }

        private static string LogbookFilename(string memberId, ulong seq)
        {
            return string.Format("{0}-{1}.logbook", Encoding.UTF8.GetBytes(memberId).Base32Encode(), seq);
        }
        
        private Dictionary<ulong, WeakReference<Logbook>> _logbooks = new Dictionary<ulong, WeakReference<Logbook>>();
        private HashSet<Logbook> _dirtyLogbooks = new HashSet<Logbook>();
        [Serialize(0)] public string MemberId { get; private set; }
        /*Serialize1*/ internal NotifyingSortedSet<ulong> _logbookSequence;
        /*Serialize2*/ private Dictionary<int, Ship> _ships;
        [Serialize(3)] public BasicInfo BasicInfo { get; private set; }
        [Serialize(4)] public Settings Settings { get; private set; }

        [Serialize(5, ConstructionType = typeof(Weekbook))]
        public ILogbook Weekbook { get; private set; }

        /*Serialize6*/ private Dictionary<Models.MapLocInfo, Models.BattleInfo> _enemyInfo;
        public Dictionary<Models.MapLocInfo, Models.BattleInfo> EnemyInfo => _enemyInfo ?? (_enemyInfo = new Dictionary<Models.MapLocInfo, Models.BattleInfo>());

        public ILogbook CurrentLogbook => Logbooks[DateTimeOffset.UtcNow.ToLogbookSequence()];
        public LogbookAccessor Logbooks { get; }

        public ShipsAccessor Ships { get; }
        public IReadOnlyCollection<ulong> LogbookSequence { get; }

        public Store(Premitives.StoragePremitive info, LinkedList<object> serializationPath) : base(info, serializationPath)
        {
            _logbookSequence.Add(0);
            Ships = new ShipsAccessor(this);
            Logbooks = new LogbookAccessor(this);
            LogbookSequence = new NotifyingReadOnlyCollectionWrapper<ulong>(_logbookSequence, _logbookSequence);
        }

        public Store(string memId)
        {
            MemberId = memId;
            _ships = new Dictionary<int, Ship>();
            _logbookSequence = new NotifyingSortedSet<ulong>();
            BasicInfo = new BasicInfo(this);
            Settings = new Settings();
            Weekbook = new Weekbook(this);

            _logbookSequence.Add(0);
            Ships = new ShipsAccessor(this);
            Logbooks = new LogbookAccessor(this);
            LogbookSequence = new NotifyingReadOnlyCollectionWrapper<ulong>(_logbookSequence, _logbookSequence);
        }

        public void SaveData()
        {
            if(!Directory.Exists(_dataDir)) {
                Directory.CreateDirectory(_dataDir);
            }

            //Purge dirty list.
            var dirtyLogbooks = _dirtyLogbooks.ToList();
            _dirtyLogbooks.Clear();

            //Write master table.
            try {
                using (Stream output = File.Open(Path.Combine(_dataDir, MasterTableFilename(MemberId)), FileMode.Create, FileAccess.Write, FileShare.None)) {
                    output.Write(_masterHeader, 0, _masterHeader.Length);
                    using (DSWriter wr = new DSWriter(output, true)) {
                        GetSerializationInfo(null).Serialize(wr);
                    }
                }
            } catch (IOException) { }

            //Write logbooks.
            foreach(var book in dirtyLogbooks) {
                try {
                    using (Stream output = File.Open(Path.Combine(_dataDir, LogbookFilename(book.MemberId, book.SequenceId)), FileMode.Create, FileAccess.Write, FileShare.None)) {
                        output.Write(_logHeader, 0, _logHeader.Length);
                        using (DSWriter wr = new DSWriter(output, true)) {
                            book.GetSerializationInfo(null).Serialize(wr);
                        }
                    }
                } catch (IOException) { }
            }
        }

        internal void RaiseBasicInfoChange()
        {
            if(_onBasicInfoChange != null) _onBasicInfoChange(this);
        }

        internal void RaiseShipDataChange(int id)
        {
            if(_onShipDataChange != null) _onShipDataChange(this, id);
        }

        protected override IReadOnlyDictionary<ulong, HandlerInfo> CustomFieldHandlers
        {
            get
            {
                return new Dictionary<ulong, HandlerInfo>() {
                    [1] = new HandlerInfo(
                        (x, p) => x.LogbookSequence.GetSerializationInfo(p, (v, p1) => new Premitives.UnsignedInteger(v)),
                        (o, i, p) => o._logbookSequence = new NotifyingSortedSet<ulong>(((Premitives.List<Premitives.UnsignedInteger>)i).Convert(x => x.Value))
                    ),
                    [2] = new HandlerInfo(
                        (x, p) => x._ships.Where(kv => kv.Value != null).GetSerializationInfo(p, (k, p1) => (Premitives.SignedInteger)(k)),
                        (o, i, p) => o._ships = (i as Premitives.Dictionary<Premitives.SignedInteger, Premitives.StoragePremitive>)?.Convert(x => (int)x.Value, x => new Ship(x, p))
                                             ?? (i as Premitives.Dictionary<Premitives.SignedInteger, Premitives.Compound>)?.Convert(x => (int)x.Value, x => new Ship(x, p))
                    ),
                    [6] = new HandlerInfo(
                        (x, p) => x._enemyInfo.GetSerializationInfo(p),
                        (o, i, p) => o._enemyInfo = ((Premitives.Dictionary<Premitives.StoragePremitive, Premitives.StoragePremitive>)i)?.Convert(x => new Models.MapLocInfo(x, p), x => new Models.BattleInfo(x, p))
                    ),
                };
            }
        }

        public class ShipsAccessor : IEnumerable<Ship>
        {
            private Store mst;
            public Ship this[int id]
            {
                get
                {
                    if(mst._ships.ContainsKey(id)) return mst._ships[id];

                    Ship s = new Ship(id, mst);
                    mst._ships.Add(id, s);
                    return s;
                }
                internal set
                {
                    mst._ships[id] = value;
                }
            }

            internal IEnumerable<int> AllIds => mst._ships.Where(x => x.Value != null).Select(x => x.Key);

            public ShipsAccessor(Store mst) { this.mst = mst; }

            public IEnumerator<Ship> GetEnumerator() { return mst._ships.Values.Where(x => x != null).GetEnumerator(); }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }

        public class LogbookAccessor
        {
            private Store _store;
            internal LogbookAccessor(Store s) { _store = s; }
            public ILogbook this[ulong seq]
            {
                get
                {
                    if(seq == 0) return _store.Weekbook;

                    WeakReference<Logbook> _reference;
                    if(_store._logbooks.TryGetValue(seq, out _reference)) {
                        Logbook _target;
                        if(_reference.TryGetTarget(out _target)) {
                            return _target;
                        }
                    }
                    Logbook newBook = LoadLogbook(_store, seq);
                    _store._logbooks[seq] = new WeakReference<Logbook>(newBook);
                    _store._logbookSequence.Add(seq);
                    _store._dirtyLogbooks.Add(newBook); //TODO Remove this.
                    return newBook;
                }
            }
        }
    }
}
