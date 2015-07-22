using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LynLogger.DataStore.Serialization;
using LynLogger.DataStore.MasterInfo;
using LynLogger.Utilities;

namespace LynLogger.DataStore.LogBook
{
    public class Ship : AbstractDSSerializable<Ship>
    {
        public string DisplayName
        {
            get
            {
                var lastInfo = ShipNameType.LastOrDefault().Value;
                return string.Format("[{0}] {1} {2}", Id, lastInfo?.TypeName, lastInfo?.ShipName);
            }
        }

        [Serialize(0)] public int Id { get; private set; }
        [Serialize(2)] public Histogram<ShipNameType> ShipNameType { get; private set; }
        [Serialize(3)] public Histogram<int> Level { get; private set; }
        [Serialize(4)] public Histogram<int> Exp { get; private set; }
        [Serialize(5)] public Histogram<int> SRate { get; private set; }

        /*
        public Histogram<int> Power { get; private set; }
        public Histogram<int> Torpedo { get; private set; }
        public Histogram<int> AntiAir { get; private set; }
        public Histogram<int> Defense { get; private set; }
        public Histogram<int> Maneuver { get; private set; }
        public Histogram<int> AntiSub { get; private set; }
        public Histogram<int> Scout { get; private set; }
        public Histogram<int> Luck { get; private set; }
        */

        [Serialize(6)] public Histogram<int> EnhancedPower { get; private set; }
        [Serialize(7)] public Histogram<int> EnhancedTorpedo { get; private set; }
        [Serialize(8)] public Histogram<int> EnhancedAntiAir { get; private set; }
        [Serialize(9)] public Histogram<int> EnhancedDefense { get; private set; }
        [Serialize(10)] public Histogram<int> EnhancedLuck { get; private set; }
        [Serialize(11)] public Histogram<ShipExistenceStatus> ExistenceLog { get; private set; }

        [Serialize(12)] public long UpdateTime { get; private set; }

        public Ship(ILogbook log, int id)
        {
            Id = id;
            ShipNameType = new Histogram<ShipNameType>(log);
            Level = new Histogram<int>(log);
            Exp = new Histogram<int>(log);
            SRate = new Histogram<int>(log);

            /*
            Power = new Histogram<int>();
            Torpedo = new Histogram<int>();
            AntiAir = new Histogram<int>();
            Defense = new Histogram<int>();
            Maneuver = new Histogram<int>();
            AntiSub = new Histogram<int>();
            Scout = new Histogram<int>();
            Luck = new Histogram<int>();
            */

            EnhancedPower = new Histogram<int>(log);
            EnhancedTorpedo = new Histogram<int>(log);
            EnhancedAntiAir = new Histogram<int>(log);
            EnhancedDefense = new Histogram<int>(log);
            EnhancedLuck = new Histogram<int>(log);

            ExistenceLog = new Histogram<ShipExistenceStatus>(log);
        }

        internal void RefreshUpdateTime() { UpdateTime = Helpers.UnixTimestamp; }

        public Ship(Premitives.StoragePremitive info, LinkedList<object> serializationPath) : base(info, serializationPath)
        {
            if(UpdateTime == 0) UpdateTime = Helpers.UnixTimestamp;
        }

        protected override IDictionary<ulong, HandlerInfo> CustomFieldHandlers
        {
            get
            {
                return new Dictionary<ulong, HandlerInfo>() {
                    [1] = HandlerInfo.NoOp,
                };
            }
        }
    }
    
    public enum ShipExistenceStatus
    {
        Existing,
        Locked,
        Sunk,
        Dismantled,
        Fused,
        NonExistence
    }
}
