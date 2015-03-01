using LynLogger.Models.Scavenge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Models
{
    [Serializable]
    public class BasicInfoHistory : IScavengable
    {
        public Histogram<int> Level { get; internal set; }
        public Histogram<int> Experience { get; internal set; }
        public Histogram<int> FurnitureCoin { get; internal set; }

        public Histogram<int> Fuel { get; internal set; }
        public Histogram<int> Ammo { get; internal set; }
        public Histogram<int> Steel { get; internal set; }
        public Histogram<int> Bauxite { get; internal set; }
        public Histogram<int> HsBuild { get; internal set; }
        public Histogram<int> HsRepair { get; internal set; }
        public Histogram<int> DevMaterial { get; internal set; }
        public Histogram<int> ModMaterial { get; internal set; }

        public Histogram<int> ExerWins { get; internal set; }
        public Histogram<int> ExerLose { get; internal set; }
        public Histogram<int> OperWins { get; internal set; }
        public Histogram<int> OperLose { get; internal set; }
        public Histogram<int> ExpeWins { get; internal set; }
        public Histogram<int> ExpeLose { get; internal set; }

        public Histogram<double> Score { get; private set; }

        public BasicInfoHistory()
        {
            Level = new Histogram<int>();
            Experience = new Histogram<int>();
            FurnitureCoin = new Histogram<int>();
            Fuel = new Histogram<int>();
            Ammo = new Histogram<int>();
            Steel = new Histogram<int>();
            Bauxite = new Histogram<int>();
            HsBuild = new Histogram<int>();
            HsRepair = new Histogram<int>();
            DevMaterial = new Histogram<int>();
            ModMaterial = new Histogram<int>();
            ExerWins = new Histogram<int>();
            ExerLose = new Histogram<int>();
            OperWins = new Histogram<int>();
            OperLose = new Histogram<int>();
            ExpeWins = new Histogram<int>();
            ExpeLose = new Histogram<int>();
            Score = new Histogram<double>();
        }

        public int Scavenge(IScavenger sc, KeyValuePair<Type, Type>[] targetTypes)
        {
            IScavengable[] scavengables = new IScavengable[] { Level, Experience, FurnitureCoin, Fuel, Ammo, Steel, Bauxite, HsBuild, HsRepair, DevMaterial, ModMaterial, ExerWins, ExerLose, OperWins, OperLose, ExpeWins, ExpeLose, Score };
            return scavengables.Select(x => x.Scavenge(sc, targetTypes)).Sum();
        }
    }
}
