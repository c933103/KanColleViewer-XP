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

        public Histogram<int> Level { get; private set; }
        public Histogram<int> Exp { get; private set; }

        public Histogram<int> SRate { get; private set; }

        /*
        public Histogram<int> Power { get; private set; }
        public Histogram<int> Torpedo { get; private set; }
        public Histogram<int> AntiAir { get; private set; }
        public Histogram<int> Defense { get; private set; }
        public Histogram<int> Maneuver { get; private set; }
        public Histogram<int> AntiSub { get; private set; }
        public Histogram<int> Scout { get; private set; }
        public Histogram<int> Luck { get; private set; }
        */

        public Histogram<int> EnhancedPower { get; private set; }
        public Histogram<int> EnhancedTorpedo { get; private set; }
        public Histogram<int> EnhancedAntiAir { get; private set; }
        public Histogram<int> EnhancedDefense { get; private set; }
        public Histogram<int> EnhancedLuck { get; private set; }

        public Histogram<ShipExistenceStatus> ExistenceLog { get; private set; }

        public ShipHistory(int id)
        {
            Id = id;
            ShipId = new Histogram<int>();
            TypeId = new Histogram<int>();
            Level = new Histogram<int>();
            Exp = new Histogram<int>();
            SRate = new Histogram<int>();

            /*
            Power = new Histogram<int>();
            Torpedo = new Histogram<int>();
            AntiAir = new Histogram<int>();
            Defense = new Histogram<int>();
            Maneuver = new Histogram<int>();
            AntiSub = new Histogram<int>();
            Scout = new Histogram<int>();
            Luck = new Histogram<int>();
            */

            EnhancedPower = new Histogram<int>();
            EnhancedTorpedo = new Histogram<int>();
            EnhancedAntiAir = new Histogram<int>();
            EnhancedDefense = new Histogram<int>();
            EnhancedLuck = new Histogram<int>();

            ExistenceLog = new Histogram<ShipExistenceStatus>();
        }
    }

    [Serializable]
    public enum ShipExistenceStatus { Existing, Locked, Sunk, Dismantled, Fused, NonExistence }
}
