using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using LynLogger.Models.Scavenge;
using LynLogger.Models.Merge;

namespace LynLogger.Models
{
    [Serializable]
    public class DataStore
    {
        public static readonly ulong StructureVersionNumber = 1;

        public static event Action<string, DataStore> OnDataStoreCreate;
        public static event Action<string, DataStore> OnDataStoreSwitch;

        public static DataStore Instance
        {
            get
            {
                if(_ds.ContainsKey(_memberId)) return _ds[_memberId];
                return null;
            }
        }

        private static readonly Dictionary<string, DataStore> _ds = new Dictionary<string, DataStore>() { { "", null } };
        private static string _memberId = "";
        private static readonly string _dataDir = Path.Combine(Environment.CurrentDirectory, "LynLogger");
        private static readonly byte[] dataFileHeader = new byte[] { 0x48, 0x41, 0x49, 0x49 };
        private static readonly byte[] compressedDataFileHeader = new byte[] { 0x48, 0x41, 0x49, 0x32 };

        private static readonly Logger.BasicInfoLogger bil = new Logger.BasicInfoLogger();
        private static readonly Logger.ShipDataLogger sdl = new Logger.ShipDataLogger();

        internal static void SwitchMember(string memberId)
        {
            var internalId = Encoding.UTF8.GetBytes(memberId).Base32Encode();
            if(_memberId == internalId && _ds.ContainsKey(_memberId)) return;

            SaveData();

            _memberId = internalId;
            if(!_ds.ContainsKey(_memberId)) {
                _ds[_memberId] = new DataStore();

                Stream input = null;
                if(File.Exists(Path.Combine(_dataDir, _memberId))) {
                    input = File.OpenRead(Path.Combine(_dataDir, _memberId));
                } else if(File.Exists(Path.Combine(_dataDir, _memberId + ".Z"))) {
                    input = File.OpenRead(Path.Combine(_dataDir, _memberId + ".Z"));
                }

                if(input != null) {
                    try {
                        _ds[_memberId] = Migrations.DataStoreLoader.LoadFromStream(input);
                    } catch(IOException) {
                    } catch(InvalidDataException) { }
                }

                _ds[_memberId].MemberId = memberId;
                _ds[_memberId].InternalMemberId = _memberId;

                if(OnDataStoreCreate != null) {
                    OnDataStoreCreate(_memberId, _ds[_memberId]);
                }
            }
            if(OnDataStoreSwitch != null) OnDataStoreSwitch(_memberId, _ds[_memberId]);
        }

        internal static void SaveData()
        {
            if(!Directory.Exists(_dataDir)) {
                Directory.CreateDirectory(_dataDir);
            }

            if(Instance == null) return;
            using(MemoryStream buf = new MemoryStream()) {
                buf.WriteVLCI(StructureVersionNumber);
                new BinaryFormatter().Serialize(buf, Instance);
                buf.Position = 0;
                using(Stream output = File.Create(Path.Combine(_dataDir, _memberId))) {
                    output.Write(dataFileHeader, 0, dataFileHeader.Length);
                    buf.CopyTo(output);
                }
                buf.Position = 0;
                using(Stream output = File.Create(Path.Combine(_dataDir, _memberId+".Z"))) {
                    output.Write(compressedDataFileHeader, 0, compressedDataFileHeader.Length);
                    Helpers.CompressData(buf, null, output);
                }
            }
        }

        public string MemberId { get; private set; }
        public string InternalMemberId { get; private set; }

        internal Dictionary<int, Models.Ship> ZwShips;
        internal Dictionary<int, Models.Ship> RwShips { get { return ZwShips; } }
        public IReadOnlyDictionary<int, Models.Ship> Ships { get { return ZwShips; } }

        internal Dictionary<int, Models.ShipHistory> ZwShipHistories;
        internal Dictionary<int, Models.ShipHistory> RwShipHistories { get { return ZwShipHistories; } }
        public IReadOnlyDictionary<int, Models.ShipHistory> ShipHistories { get { return ZwShipHistories; } }

        [field: NonSerialized]
        public event Action<int> ShipDataChanged;
        internal void RaiseShipDataChange(int id) { if(ShipDataChanged != null) ShipDataChanged(id); }

        internal Models.BasicInfo ZwBasicInfo;
        public Models.BasicInfo BasicInfo { get { return ZwBasicInfo; } }

        internal Models.BasicInfoHistory ZwBasicInfoHistory;
        public Models.BasicInfoHistory BasicInfoHistory { get { return ZwBasicInfoHistory; } }

        [field: NonSerialized]
        public event Action BasicInfoChanged;
        internal void RaiseBasicInfoChange() { if(BasicInfoChanged != null) BasicInfoChanged(); }

        internal Models.Settings ZwSettings;
        public Models.Settings Settings { get { return ZwSettings; } }

        private DataStore()
        {
            ZwShips = new Dictionary<int, Models.Ship>();
            ZwBasicInfo = new Models.BasicInfo();

            ZwShipHistories = new Dictionary<int, Models.ShipHistory>();
            ZwBasicInfoHistory = new Models.BasicInfoHistory();

            ZwSettings = new Models.Settings();
        }

        internal DataStore(string memberId, string internalMemberId)
        {
            MemberId = memberId;
            InternalMemberId = internalMemberId;
        }

        public int Cleanup(Scavenge.IScavenger sc)
        {
            var targetTypes = Attribute.GetCustomAttributes(sc.GetType(), typeof(Scavenge.ScavengeTargetTypeAttribute)).Cast<Scavenge.ScavengeTargetTypeAttribute>().Select(x => new KeyValuePair<Type, Type>(x.TargetKeyType, x.TargetValueType)).ToArray();
            var scavengeCount = 0;
            scavengeCount += RwShipHistories.Scavenge(sc, targetTypes, false);
            scavengeCount += BasicInfoHistory.Scavenge(sc, targetTypes);
            return scavengeCount;
        }

        public void Merge(DataStore ds)
        {
            RwShipHistories.Merge(ds.RwShipHistories);
            RwShips.Merge(ds.RwShips);
            BasicInfoHistory.Merge(ds.BasicInfoHistory);
        }
    }
}
