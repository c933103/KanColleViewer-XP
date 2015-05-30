using LynLogger.DataStore.MasterInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Models.Battling
{
    public class PracticeEnemyInfo
    {
        internal EnemyShipInfo[] ZwEnemyShips;
        internal string ZwRawData;

        public IReadOnlyList<EnemyShipInfo> EnemyShips { get { return ZwEnemyShips; } }
        public string RawData { get { return ZwRawData; } }

        public struct EnemyShipInfo
        {
            internal int ZwId;
            internal int ZwLevel;
            internal ShipNameType ZwShipType;
            internal int ZwStar;

            public int Id { get { return ZwId; } }
            public int Level { get { return ZwLevel; } }
            public ShipNameType ShipType { get { return ZwShipType; } }
            public int Star { get { return ZwStar; } }
        }
    }
}
