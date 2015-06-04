using LynLogger.DataStore.Serialization;
using LynLogger.Models.Battling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.DataStore.LogBook
{
    public class DrillInfo : AbstractDSSerializable<DrillInfo>
    {
        [Serialize(0)] public PracticeEnemyInfo Briefing { get; set; }
        [Serialize(1)] public BattleProcess Battle { get; set; }
        [Serialize(2)] public BattleResult Result { get; set; }
        
        public DrillInfo() { }
        public DrillInfo(Premitives.StoragePremitive _info, LinkedList<object> _path) : base(_info, _path) { }
    }
}
