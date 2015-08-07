using Grabacr07.KanColleWrapper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LynLogger.DataStore.Serialization;

namespace LynLogger.DataStore.MasterInfo
{
    [Serializable]
    public class EquiptInfo : AbstractDSSerializable<EquiptInfo>, ICloneable
    {
        [Serialize(0)] public int Id { get; private set; }
        [Serialize(1)] public int EquiptId { get; private set; }
        [Serialize(2)] public int EquiptCount { get; private set; }
        [Serialize(3)] public int Level { get; private set; }
        [Serialize(4)] public int AsControl { get; private set; }
        [Serialize(5)] public string EquiptName { get; private set; }

        public int SlotAsControl => (int)(Math.Sqrt(EquiptCount) * AsControl);

        public EquiptInfo(SlotItemInfo info, int count, int id = 0)
        {
            if(info != null) {
                Level = 0;
                AsControl = info.IsAirSuperiorityFighter ? info.AA : 0;
                EquiptId = info.Id;
                EquiptName = info.Name;
            } else {
                AsControl = Level = 0;
                EquiptId = id;
                EquiptName = id.ToString();
            }
            Id = id;
            EquiptCount = count;
        }

        public EquiptInfo(SlotItem si, int count, int id = 0)
        {
            if(si != null) {
                Id = si.Id;
                EquiptId = si.Info.Id;
                EquiptName = si.Info.Name;
                Level = si.Level;
                AsControl = si.Info.IsAirSuperiorityFighter ? si.Info.AA : 0;
            } else {
                Id = EquiptId = id;
                AsControl = Level = 0;
                EquiptName = id.ToString();
            }
            EquiptCount = count;
        }

        public EquiptInfo(ShipSlot ss, int id = 0, int count = 0)
        {
            if(ss != null) {
                Id = ss.Item.Id;
                EquiptId = ss.Item.Info.Id;
                EquiptName = ss.Item.Info.Name;
                Level = ss.Item.Level;
                AsControl = ss.Item.Info.IsAirSuperiorityFighter ? ss.Item.Info.AA : 0;
                EquiptCount = ss.Current;
            } else {
                Id = EquiptId = id;
                AsControl = Level = 0;
                EquiptCount = count;
                EquiptName = id.ToString();
            }
        }
        public EquiptInfo(Premitives.StoragePremitive info, LinkedList<object> serializationPath) : base(info, serializationPath) { }

        public EquiptInfo(int r, int c) : this(Grabacr07.KanColleWrapper.KanColleClient.Current.Master.SlotItems[r], c, r)
        {
        }

        object ICloneable.Clone() { return MemberwiseClone(); }
    }
}
