using LynLogger.Models.Merge;
using LynLogger.Models.Scavenge;
using LynLogger.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace LynLogger.Models
{
    [Serializable]
    public class DataStore
    {
        public static readonly ulong StructureVersionNumber = 1;

        private static Action<string, DataStore> _onDataStoreCreate;
        private static Action<string, DataStore> _onDataStoreSwitch;

        public static event Action<string, DataStore> OnDataStoreCreate
        {
            add { _onDataStoreCreate += value.MakeWeak(x => _onDataStoreCreate -= x); }
            remove { }
        }

        public static event Action<string, DataStore> OnDataStoreSwitch
        {
            add { _onDataStoreSwitch += value.MakeWeak(x => _onDataStoreSwitch -= x); }
            remove { }
        }

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
                
                if(_onDataStoreCreate != null) _onDataStoreCreate(_memberId, _ds[_memberId]);
            }
            if(_onDataStoreSwitch != null) _onDataStoreSwitch(_memberId, _ds[_memberId]);
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

        internal Dictionary<int, Ship> ZwShips;
        internal Dictionary<int, Ship> RwShips { get { return ZwShips; } }
        public IReadOnlyDictionary<int, Ship> Ships { get { return ZwShips; } }

        internal Dictionary<int, ShipHistory> ZwShipHistories;
        internal Dictionary<int, ShipHistory> RwShipHistories { get { return ZwShipHistories; } }
        public IReadOnlyDictionary<int, ShipHistory> ShipHistories { get { return ZwShipHistories; } }
        
        //[NonSerialized]
        internal void RaiseShipDataChange(int id) { if(_shipDataChanged != null) _shipDataChanged(this, id); }
        private static Action<DataStore, int> _shipDataChanged;
        public static event Action<DataStore, int> ShipDataChanged
        {
            add { _shipDataChanged += value.MakeWeak(x => _shipDataChanged -= x); }
            remove { }
        }

        internal BasicInfo ZwBasicInfo;
        public BasicInfo BasicInfo { get { return ZwBasicInfo; } }

        internal BasicInfoHistory ZwBasicInfoHistory;
        public BasicInfoHistory BasicInfoHistory { get { return ZwBasicInfoHistory; } }

        //[NonSerialized]
        internal void RaiseBasicInfoChange() { if(_basicInfoChanged != null) _basicInfoChanged(this); }
        private static Action<DataStore> _basicInfoChanged;
        public static event Action<DataStore> BasicInfoChanged
        {
            add { _basicInfoChanged += value.MakeWeak(x => _basicInfoChanged -= x); }
            remove { }
        }

        internal Settings ZwSettings;
        public Settings Settings { get { return ZwSettings; } }

        private DataStore()
        {
            ZwShips = new Dictionary<int, Ship>();
            ZwBasicInfo = new BasicInfo();

            ZwShipHistories = new Dictionary<int, ShipHistory>();
            ZwBasicInfoHistory = new BasicInfoHistory();

            ZwSettings = new Settings();
        }

        internal DataStore(string memberId, string internalMemberId)
        {
            MemberId = memberId;
            InternalMemberId = internalMemberId;
        }

        public int Cleanup(Scavenge.IScavenger sc)
        {
            var targetTypes = Attribute.GetCustomAttributes(sc.GetType(), typeof(ScavengeTargetTypeAttribute)).Cast<ScavengeTargetTypeAttribute>().Select(x => new KeyValuePair<Type, Type>(x.TargetKeyType, x.TargetValueType)).ToArray();
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
