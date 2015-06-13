using LynLogger.DataStore.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShipInfo = LynLogger.Models.Battling.BattleProcess.ShipInfo;
using Formation = LynLogger.Models.Battling.BattleProcess.Formation;
using LynLogger.DataStore.Premitives;
using LynLogger.DataStore.Extensions;

namespace LynLogger.Models
{
    public struct BattleInfo : IDSSerializable
    {
        public Formation Formation { get; set; }
        internal ShipInfo[] _enemyShips;
        public int AdmiralExp { get; set; }
        public int BaseExp { get; set; }

        public IList<ShipInfo> EnemyShips => _enemyShips;

        public StoragePremitive GetSerializationInfo(LinkedList<object> _path)
        {
            return new Compound() {
                [16] = (SignedInteger)(int)Formation,
                [17] = _enemyShips.GetSerializationInfo(_path),
                [18] = (SignedInteger)AdmiralExp,
                [19] = (SignedInteger)BaseExp,
            };
        }

        public BattleInfo(StoragePremitive info, LinkedList<object> serializationPath)
        {
            var comp = (Compound)info;
            Formation = (Formation)(int)(SignedInteger)comp[16];
            _enemyShips = ((DataStore.Premitives.List<StoragePremitive>)comp[17]).Convert((x) => new ShipInfo(x, serializationPath)).ToArray();
            AdmiralExp = (int)(SignedInteger)comp[18];
            BaseExp = (int)(SignedInteger)comp[19];
        }
    }
}
