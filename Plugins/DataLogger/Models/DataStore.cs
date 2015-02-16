using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace LynLogger
{
    [Serializable]
    public class DataStore
    {
        public static event Action<string, DataStore> OnDataStoreCreate;

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

        private static readonly System.Runtime.Serialization.Formatters.Binary.BinaryFormatter Serializer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        private static readonly byte[] _trainningData = Properties.Resources.TrainningData;

        private static readonly Logger.BasicInfoLogger bil = new Logger.BasicInfoLogger();
        private static readonly Logger.ShipDataLogger sdl = new Logger.ShipDataLogger();

        internal static void SwitchMember(string memberId)
        {
            SaveData();

            _memberId = Encoding.UTF8.GetBytes(memberId).Base32Encode();
            if(_ds.ContainsKey(_memberId)) return;

            if(!File.Exists(Path.Combine(_dataDir, _memberId))) {
                if(!File.Exists(Path.Combine(_dataDir, _memberId + ".Z"))) {
                    _ds[_memberId] = new DataStore();
                } else {
                    using(Stream input = File.OpenRead(Path.Combine(_dataDir, _memberId + ".Z"))) {
                        using(MemoryStream buf = new MemoryStream()) {
                            Helpers.DecompressData(input, _trainningData, buf);
                            buf.Position = 0;
                            _ds[_memberId] = (DataStore)Serializer.Deserialize(buf);
                        }
                    }
                }
            } else {
                using(Stream input = File.OpenRead(Path.Combine(_dataDir, _memberId))) {
                    _ds[_memberId] = (DataStore)Serializer.Deserialize(input);
                }
            }
            _ds[_memberId].MemberId = memberId;
            _ds[_memberId].InternalMemberId = _memberId;

            if(OnDataStoreCreate != null) {
                OnDataStoreCreate(_memberId, _ds[_memberId]);
            }
        }

        internal static void SaveData()
        {
            if(!Directory.Exists(_dataDir)) {
                Directory.CreateDirectory(_dataDir);
            }

            if(Instance == null) return;
            using(MemoryStream buf = new MemoryStream()) {
                Serializer.Serialize(buf, Instance);
                buf.Position = 0;
                using(Stream output = File.OpenWrite(Path.Combine(_dataDir, _memberId))) {
                    buf.CopyTo(output);
                }
                buf.Position = 0;
                using(Stream output = File.OpenWrite(Path.Combine(_dataDir, _memberId+".Z"))) {
                    Helpers.CompressData(buf, _trainningData, output);
                }
            }
        }

        public string MemberId { get; private set; }
        public string InternalMemberId { get; private set; }
        public int RecordVersion { get; private set; }

        public IReadOnlyDictionary<int, Models.Ship> Ships { get { return i_Ships; } }
        public IReadOnlyDictionary<int, Models.ShipHistory> ShipHistories { get { return i_ShipHistories; } }

        internal Dictionary<int, Models.Ship> i_Ships { get; private set; }
        internal Dictionary<int, Models.ShipHistory> i_ShipHistories { get; private set; }

        [NonSerialized] private Action<int> _shipDataChanged;
        public event Action<int> ShipDataChanged { add { _shipDataChanged += value; } remove { _shipDataChanged -= value; } }
        internal void RaiseShipDataChange(int id) { if(_shipDataChanged != null) _shipDataChanged(id); }

        public Models.BasicInfo BasicInfo { get; private set; }
        public Models.BasicInfoHistory BasicInfoHistory { get; private set; }

        [NonSerialized] private Action _basicInfoChanged;
        public event Action BasicInfoChanged { add { _basicInfoChanged += value; } remove { _basicInfoChanged -= value; } }
        internal void RaiseBasicInfoChange() { if(_basicInfoChanged != null) _basicInfoChanged(); }

        private DataStore()
        {
            i_Ships = new Dictionary<int, Models.Ship>();
            BasicInfo = new Models.BasicInfo();

            i_ShipHistories = new Dictionary<int, Models.ShipHistory>();
            BasicInfoHistory = new Models.BasicInfoHistory();

            RecordVersion = 1;
        }
    }
}
