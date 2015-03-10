using Grabacr07.KanColleWrapper.Models;
using System;

namespace LynLogger.Models
{
    [Serializable]
    public class EquiptInfo
    {
        public int Id { get; private set; }

        internal int ZwEquiptId;
        public int EquiptId { get { return ZwEquiptId; } }

        internal int ZwEquiptCount;
        public int EquiptCount { get { return ZwEquiptCount; } }

        internal int ZwLevel;
        public int Level { get { return ZwLevel; } }

        internal string ZwEquiptName;
        public string EquiptName { get { return ZwEquiptName; } }

        public EquiptInfo(int id)
        {
            Id = id;
        }

        public EquiptInfo(SlotItem si, int count, int id = -1)
        {
            if(si != null) {
                Id = si.Id;
                ZwEquiptId = si.Info.Id;
                ZwEquiptName = si.Info.Name;
                ZwLevel = si.Level;
            } else {
                Id = id;
                ZwEquiptId = id;
                ZwEquiptName = "";
                ZwLevel = 0;
            }
            ZwEquiptCount = count;
        }
    }
}
