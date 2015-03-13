using Grabacr07.KanColleWrapper.Models;
using System;

namespace LynLogger.Models
{
    [Serializable]
    public struct EquiptInfo
    {
        public int Id { get; private set; }

        internal int ZwEquiptId;
        public int EquiptId { get { return ZwEquiptId; } }

        internal int ZwEquiptCount;
        public int EquiptCount { get { return ZwEquiptCount; } }

        internal int ZwLevel;
        public int Level { get { return ZwLevel; } }

        [System.Runtime.Serialization.OptionalField] internal int ZwAsControl;
        public int AsControl { get { return ZwAsControl; } }

        internal string ZwEquiptName;
        public string EquiptName { get { return ZwEquiptName; } }

        public int SlotAsControl { get { return (int)(Math.Sqrt(EquiptCount) * AsControl); } }
        
        public EquiptInfo(SlotItemInfo info, int count, int id = 0)
        {
            if(info != null) {
                ZwLevel = 0;
                ZwAsControl = info.IsAirSuperiorityFighter ? info.AA : 0;
                ZwEquiptId = info.Id;
                ZwEquiptName = info.Name;
            } else {
                ZwAsControl = ZwLevel = 0;
                ZwEquiptId = id;
                ZwEquiptName = id.ToString();
            }
            Id = id;
            ZwEquiptCount = count;
        }

        public EquiptInfo(SlotItem si, int count, int id = 0)
        {
            if(si != null) {
                Id = si.Id;
                ZwEquiptId = si.Info.Id;
                ZwEquiptName = si.Info.Name;
                ZwLevel = si.Level;
                ZwAsControl = si.Info.IsAirSuperiorityFighter ? si.Info.AA : 0;
            } else {
                Id = ZwEquiptId = id;
                ZwAsControl = ZwLevel = 0;
                ZwEquiptName = id.ToString();
            }
            ZwEquiptCount = count;
        }

        public EquiptInfo(ShipSlot ss, int id = 0, int count = 0)
        {
            if(ss != null) {
                Id = ss.Item.Id;
                ZwEquiptId = ss.Item.Info.Id;
                ZwEquiptName = ss.Item.Info.Name;
                ZwLevel = ss.Item.Level;
                ZwAsControl = ss.Item.Info.IsAirSuperiorityFighter ? ss.Item.Info.AA : 0;
                ZwEquiptCount = ss.Current;
            } else {
                Id = ZwEquiptId = id;
                ZwAsControl = ZwLevel = 0;
                ZwEquiptCount = count;
                ZwEquiptName = id.ToString();
            }
        }
    }
}
