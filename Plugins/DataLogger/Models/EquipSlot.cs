using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Models
{
    [Serializable]
    public struct EquipSlot
    {
        public int EquidId { get; internal set; }
        public int Capacity { get; internal set; }
    }
}
