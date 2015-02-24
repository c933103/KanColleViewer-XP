using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Models.Migrations.Versions.V0
{
    [Serializable]
    public class DataStore : IMigratableDataStore
    {
        public string MemberId { get; private set; }
        public string InternalMemberId { get; private set; }

        internal Dictionary<int, Models.Ship> ZwShips;
        internal Dictionary<int, Models.ShipHistory> ZwShipHistories;

        internal Models.BasicInfo ZwBasicInfo;
        internal Models.BasicInfoHistory ZwBasicInfoHistory;

        internal Models.Settings ZwSettings;

        public Models.DataStore Migrate()
        {
            return new Models.DataStore(MemberId, InternalMemberId) {
                ZwBasicInfo = ZwBasicInfo,
                ZwBasicInfoHistory = ZwBasicInfoHistory,
                ZwShipHistories = ZwShipHistories,
                ZwShips = ZwShips,
                ZwSettings = ZwSettings
            };
        }
    }
}
