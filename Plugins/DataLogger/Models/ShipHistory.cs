using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Models
{
    [Serializable]
    public class ShipHistory
    {
        public int Id { get; private set; }
        public Histogram<int> ShipId { get; private set; }
        public Histogram<int> TypeId { get; private set; }

        public Histogram<int> Level { get; internal set; }
        public Histogram<int> Exp { get; internal set; }

        public Histogram<int> SRate { get; internal set; }

        public Histogram<int> Power { get; internal set; }
        public Histogram<int> Torpedo { get; internal set; }
        public Histogram<int> AntiAir { get; internal set; }
        public Histogram<int> Defense { get; internal set; }
        public Histogram<int> Maneuver { get; internal set; }
        public Histogram<int> AntiSub { get; internal set; }
        public Histogram<int> Scout { get; internal set; }
        public Histogram<int> Luck { get; internal set; }

        public Histogram<int> EnhancedPower { get; internal set; }
        public Histogram<int> EnhancedTorpedo { get; internal set; }
        public Histogram<int> EnhancedAntiAir { get; internal set; }
        public Histogram<int> EnhancedDefense { get; internal set; }
        public Histogram<int> EnhancedLuck { get; internal set; }

        public Histogram<ShipExistenceStatus> ExistenceLog { get; internal set; }
    }

    [Serializable]
    public enum ShipExistenceStatus { Existing, Locked, Sunk, Dismantled, Fused, NonExistence }
}
