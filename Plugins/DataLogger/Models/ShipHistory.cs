using LynLogger.Models.Merge;
using LynLogger.Models.Scavenge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace LynLogger.Models
{
    [Serializable]
    public class ShipHistory : IScavengable, IMergable<ShipHistory>
    {
        public string DisplayName
        {
            get
            {
                var lastInfo = ShipNameType.Last().Value;
                return string.Format("[{0}] {1} {2}", Id, lastInfo.TypeName, lastInfo.ShipName);
            }
        }

        public int Id { get; private set; }

        private Histogram<string> ZwShipName;
        private Histogram<string> ZwTypeName;
        private Histogram<int> ZwShipId;
        [OptionalField] internal Histogram<ShipNameType> ZwShipNameType;
        
        public Histogram<ShipNameType> ShipNameType
        {
            get
            {
                if(ZwShipNameType == null) {
                    ZwShipNameType = new Histogram<Models.ShipNameType>(Helpers.Zip(ZwShipId, ZwShipName, ZwTypeName, (id, sn, tn) =>
                        new KeyValuePair<long, ShipNameType>(id.Key, new Models.ShipNameType {
                            ShipId = id.Value,
                            ShipName = sn.Value,
                            TypeName = tn.Value
                        }
                    )));
                    ZwShipId = null;
                    ZwShipName = ZwTypeName = null;
                }
                return ZwShipNameType;
            }
        }

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
            ZwShipNameType = new Histogram<ShipNameType>();
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

        public int Scavenge(IScavenger sc, KeyValuePair<Type, Type>[] targetTypes)
        {
            IScavengable[] scavengables = new IScavengable[] { ShipNameType, ZwLevel, ZwExp, ZwSRate, ZwEnhancedPower, ZwEnhancedTorpedo, ZwEnhancedAntiAir, ZwEnhancedDefense, ZwEnhancedLuck, ZwExistenceLog };
            return scavengables.Select(x => x.Scavenge(sc, targetTypes)).Sum();
        }

        public void Merge(ShipHistory val)
        {
            ShipNameType.Merge(val.ShipNameType);
            ZwLevel.Merge(val.ZwLevel);
            ZwExp.Merge(val.ZwExp);
            ZwSRate.Merge(val.ZwSRate);
            ZwEnhancedPower.Merge(val.ZwEnhancedPower);
            ZwEnhancedTorpedo.Merge(val.ZwEnhancedTorpedo);
            ZwEnhancedAntiAir.Merge(val.ZwEnhancedAntiAir);
            ZwEnhancedDefense.Merge(val.ZwEnhancedDefense);
            ZwEnhancedLuck.Merge(val.ZwEnhancedLuck);
            ZwExistenceLog.Merge(val.ZwExistenceLog);
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
