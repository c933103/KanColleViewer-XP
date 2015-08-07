using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LynLogger.DataStore.Serialization;

namespace LynLogger.DataStore.LogBook
{
    public class BasicInfo : AbstractDSSerializable<BasicInfo>
    {
        [Serialize(0)] public Histogram<int> Level { get; internal set; }
        [Serialize(1)] public Histogram<int> Experience { get; internal set; }
        [Serialize(2)] public Histogram<int> FurnitureCoin { get; internal set; }

        [Serialize(3)] public Histogram<int> Fuel { get; internal set; }
        [Serialize(4)] public Histogram<int> Ammo { get; internal set; }
        [Serialize(5)] public Histogram<int> Steel { get; internal set; }
        [Serialize(6)] public Histogram<int> Bauxite { get; internal set; }
        [Serialize(7)] public Histogram<int> HsBuild { get; internal set; }
        [Serialize(8)] public Histogram<int> HsRepair { get; internal set; }
        [Serialize(9)] public Histogram<int> DevMaterial { get; internal set; }
        [Serialize(10)] public Histogram<int> ModMaterial { get; internal set; }

        [Serialize(11)] public Histogram<int> ExerWins { get; internal set; }
        [Serialize(12)] public Histogram<int> ExerLose { get; internal set; }
        [Serialize(13)] public Histogram<int> OperWins { get; internal set; }
        [Serialize(14)] public Histogram<int> OperLose { get; internal set; }
        [Serialize(15)] public Histogram<int> ExpeWins { get; internal set; }
        [Serialize(16)] public Histogram<int> ExpeLose { get; internal set; }

        public BasicInfo(ILogbook log)
        {
            Level = new Histogram<int>(log);
            Experience = new Histogram<int>(log);
            FurnitureCoin = new Histogram<int>(log);
            Fuel = new Histogram<int>(log);
            Ammo = new Histogram<int>(log);
            Steel = new Histogram<int>(log);
            Bauxite = new Histogram<int>(log);
            HsBuild = new Histogram<int>(log);
            HsRepair = new Histogram<int>(log);
            DevMaterial = new Histogram<int>(log);
            ModMaterial = new Histogram<int>(log);
            ExerWins = new Histogram<int>(log);
            ExerLose = new Histogram<int>(log);
            OperWins = new Histogram<int>(log);
            OperLose = new Histogram<int>(log);
            ExpeWins = new Histogram<int>(log);
            ExpeLose = new Histogram<int>(log);
        }

        public BasicInfo(Premitives.StoragePremitive info, LinkedList<object> serializationPath) : base(info, serializationPath) { }
    }
}
