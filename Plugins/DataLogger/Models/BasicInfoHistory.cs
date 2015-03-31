using LynLogger.Models.Merge;
using LynLogger.Models.Scavenge;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LynLogger.Models
{
    [Serializable]
    public class BasicInfoHistory : IScavengable, IMergable<BasicInfoHistory>
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
        }

        public int Scavenge(IScavenger sc, KeyValuePair<Type, Type>[] targetTypes)
        {
            IScavengable[] scavengables = new IScavengable[] { Level, Experience, FurnitureCoin, Fuel, Ammo, Steel, Bauxite, HsBuild, HsRepair, DevMaterial, ModMaterial, ExerWins, ExerLose, OperWins, OperLose, ExpeWins, ExpeLose };
            return scavengables.Select(x => x.Scavenge(sc, targetTypes)).Sum();
        }

        public void Merge(BasicInfoHistory val)
        {
            Level.Merge(val.Level);
            Experience.Merge(val.Experience);
            FurnitureCoin.Merge(val.FurnitureCoin);
            Fuel.Merge(val.Fuel);
            Ammo.Merge(val.Ammo);
            Steel.Merge(val.Steel);
            Bauxite.Merge(val.Bauxite);
            HsBuild.Merge(val.HsBuild);
            HsRepair.Merge(val.HsRepair);
            DevMaterial.Merge(val.DevMaterial);
            ModMaterial.Merge(val.ModMaterial);
            ExerWins.Merge(val.ExerWins);
            ExerLose.Merge(val.ExerLose);
            OperWins.Merge(val.OperWins);
            OperLose.Merge(val.OperLose);
            ExpeWins.Merge(val.ExpeWins);
            ExpeLose.Merge(val.ExpeLose);
        }
    }
}
