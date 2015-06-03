using LynLogger.DataStore.Extensions;
using LynLogger.DataStore.MasterInfo;
using LynLogger.DataStore.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Models.Battling
{
    public class PracticeEnemyInfo : AbstractDSSerializable<PracticeEnemyInfo>
    {
        [Serialize(0)] internal int ZwEnemyLevel;
        [Serialize(1)] internal string ZwEnemyName;
        [Serialize(2)] internal string ZwEnemyFleetName;
        /*[Serialize(3)]*/ internal EnemyShipInfo[] ZwEnemyShips;
        [Serialize(4, DepthLimit = 2)] internal string ZwRawData;

        public int EnemyLevel => ZwEnemyLevel;
        public string EnemyName => ZwEnemyName;
        public string EnemyFleetName => ZwEnemyFleetName;
        public IReadOnlyList<EnemyShipInfo> EnemyShips => ZwEnemyShips;
        public string RawData => ZwRawData;

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

        internal PracticeEnemyInfo() { }
        internal PracticeEnemyInfo(DataStore.Premitives.StoragePremitive x, LinkedList<object> path) : base(x, path) { }
        protected override IReadOnlyDictionary<ulong, HandlerInfo> CustomFieldHandlers
        {
            get
            {
                return new Dictionary<ulong, HandlerInfo>() {
                    [3] = new HandlerInfo(
                        (x, p) => x.ZwEnemyShips.GetSerializationInfo(p),
                        (o, i, p) => o.ZwEnemyShips = ((DataStore.Premitives.List<DataStore.Premitives.StoragePremitive>)i).Convert(x => new EnemyShipInfo(x, p)).ToArray()),
                };
            }
        }

        public class EnemyShipInfo : AbstractDSSerializable<EnemyShipInfo>
        {
            [Serialize(0)] internal int ZwId;
            [Serialize(1)] internal int ZwLevel;
            [Serialize(2)] internal ShipNameType ZwShipType;
            [Serialize(3)] internal int ZwStar;

            public int Id => ZwId;
            public int Level => ZwLevel;
            public ShipNameType ShipType => ZwShipType;
            public int Star => ZwStar;

            internal EnemyShipInfo() { }
            internal EnemyShipInfo(DataStore.Premitives.StoragePremitive x, LinkedList<object> path) : base(x, path) { }
        }
    }
}
