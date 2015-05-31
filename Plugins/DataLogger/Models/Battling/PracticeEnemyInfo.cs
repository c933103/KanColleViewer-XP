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

        public IList<EnemyShipInfo> EnemyShips { get { return ZwEnemyShips; } }
        public string RawData { get { return ZwRawData; } }

        public int DrillBasicExp
        {
            get
            {
                var bi = Data.LevelExperienceTable.Accumulated[EnemyShips[0].Level] / 100;
                if(EnemyShips.Count > 1) {
                    bi += Data.LevelExperienceTable.Accumulated[EnemyShips[1].Level] / 300;
                }
                if(bi > 500) {
                    bi = 500 + (int)Math.Sqrt(bi - 500);
                }
                return bi;
            }
        }

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
