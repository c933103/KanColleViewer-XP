using System;
using System.Collections.Generic;
using System.Linq;

namespace LynLogger.Models.Migrations.Versions.V0
{
    [Serializable]
    public class ShipHistory
    {
        public int Id { get; private set; }
        public Histogram<int> ShipId { get; private set; }
        public Histogram<int> TypeId { get; private set; }

        public Histogram<int> Level { get; private set; }
        public Histogram<int> Exp { get; private set; }

        public Histogram<int> SRate { get; private set; }

        public Histogram<int> EnhancedPower { get; private set; }
        public Histogram<int> EnhancedTorpedo { get; private set; }
        public Histogram<int> EnhancedAntiAir { get; private set; }
        public Histogram<int> EnhancedDefense { get; private set; }
        public Histogram<int> EnhancedLuck { get; private set; }

        public Histogram<ShipExistenceStatus> ExistenceLog { get; private set; }

        public Models.ShipHistory Migrate()
        {
            return new Models.ShipHistory(Id) {
                ZwShipNameType = new Histogram<ShipNameType>(ShipId.Select(x => new KeyValuePair<long, ShipNameType>(x.Key, Helpers.LookupShipNameInfo(x.Value)))),
                ZwEnhancedAntiAir = EnhancedAntiAir,
                ZwEnhancedDefense = EnhancedDefense,
                ZwEnhancedLuck = EnhancedLuck,
                ZwEnhancedPower = EnhancedPower,
                ZwEnhancedTorpedo = EnhancedTorpedo,
                ZwExistenceLog = ExistenceLog,
                ZwExp = Exp,
                ZwLevel = Level,
                ZwSRate = SRate,
            };
        }
    }
}
