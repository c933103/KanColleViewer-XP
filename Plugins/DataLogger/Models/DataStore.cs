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
        private static readonly SevenZip.Compression.LZMA.Encoder Compressor = new SevenZip.Compression.LZMA.Encoder();
        private static readonly SevenZip.Compression.LZMA.Decoder Decompressor = new SevenZip.Compression.LZMA.Decoder();
        private static readonly byte[] _trainningData = Encoding.UTF8.GetBytes(Properties.Resources.TrainningString);

        public static event Action<string, DataStore> OnDataStoreCreate;

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

        private static int DetermineDictionarySize(long dataLen)
        {
            var totalLen = _trainningData.Length + dataLen;
            for(int i = 10; i < 24; i++) {
                if(totalLen < (1 << i)) return i;
            }
            return 24;
        }

        public IReadOnlyDictionary<int, Models.Ship> Ships { get { return i_Ships; } }
        internal Dictionary<int, Models.Ship> i_Ships { get; private set; }

        [NonSerialized] private Action<int> _shipDataChanged;
        public event Action<int> ShipDataChanged { add { _shipDataChanged += value; } remove { _shipDataChanged -= value; } }
        internal void RaiseShipDataChange(int id) { if(_shipDataChanged != null) _shipDataChanged(id); }

        public Models.BasicInfo BasicInfo { get; private set; }

        [NonSerialized] private Action _basicInfoChanged;
        public event Action BasicInfoChanged { add { _basicInfoChanged += value; } remove { _basicInfoChanged -= value; } }
        internal void RaiseBasicInfoChange() { if(_basicInfoChanged != null) _basicInfoChanged(); }

        private DataStore()
        {
            i_Ships = new Dictionary<int, Models.Ship>();
            BasicInfo = new Models.BasicInfo();
        }
    }
}
