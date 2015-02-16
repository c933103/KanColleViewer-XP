using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Models
{
    [Serializable]
    public class BasicInfoHistory
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
    }
}
