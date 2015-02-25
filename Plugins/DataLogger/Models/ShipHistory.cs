using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Models
{
    [Serializable]
    public class ShipHistory
    {
        public string DisplayName
        {
            get
            {
                return string.Format("[{0}] {1} {2}", Id, TypeName.Last().Value, ShipName.Last().Value);
            }
        }

        public int Id { get; private set; }

        internal Histogram<string> ZwShipName;
        internal Histogram<string> ZwTypeName;
        internal Histogram<int> ZwShipId;
        internal Histogram<int> ZwTypeId;

        public Histogram<string> ShipName { get { return ZwShipName; } }
        public Histogram<string> TypeName { get { return ZwTypeName; } }
        public Histogram<int> ShipId { get { return ZwShipId; } }
        public Histogram<int> TypeId { get { return ZwTypeId; } }

        internal Histogram<int> ZwLevel;
        internal Histogram<int> ZwExp;

        public Histogram<int> Level { get { return ZwLevel; } }
        public Histogram<int> Exp { get { return ZwExp; } }

        internal Histogram<int> ZwSRate;
        public Histogram<int> SRate { get { return ZwSRate; } }

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

        internal Histogram<int> ZwEnhancedPower;
        internal Histogram<int> ZwEnhancedTorpedo;
        internal Histogram<int> ZwEnhancedAntiAir;
        internal Histogram<int> ZwEnhancedDefense;
        internal Histogram<int> ZwEnhancedLuck;

        public Histogram<int> EnhancedPower { get { return ZwEnhancedPower; } }
        public Histogram<int> EnhancedTorpedo { get { return ZwEnhancedTorpedo; } }
        public Histogram<int> EnhancedAntiAir { get { return ZwEnhancedAntiAir; } }
        public Histogram<int> EnhancedDefense { get { return ZwEnhancedDefense; } }
        public Histogram<int> EnhancedLuck { get { return ZwEnhancedLuck; } }

        internal Histogram<ShipExistenceStatus> ZwExistenceLog;
        public Histogram<ShipExistenceStatus> ExistenceLog { get { return ZwExistenceLog; } }

        public ShipHistory(int id)
        {
            Id = id;
            ZwShipName = new Histogram<string>();
            ZwTypeName = new Histogram<string>();
            ZwShipId = new Histogram<int>();
            ZwTypeId = new Histogram<int>();
            ZwLevel = new Histogram<int>();
            ZwExp = new Histogram<int>();
            ZwSRate = new Histogram<int>();

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

            ZwEnhancedPower = new Histogram<int>();
            ZwEnhancedTorpedo = new Histogram<int>();
            ZwEnhancedAntiAir = new Histogram<int>();
            ZwEnhancedDefense = new Histogram<int>();
            ZwEnhancedLuck = new Histogram<int>();

            ZwExistenceLog = new Histogram<ShipExistenceStatus>();
        }
    }

    [Serializable]
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
