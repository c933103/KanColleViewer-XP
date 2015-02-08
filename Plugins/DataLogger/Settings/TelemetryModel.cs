using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.ViewModels;
using DataLogger.Properties;

namespace DataLogger.Settings
{
    public class TelemetryModel : TabItemViewModel
    {
        public override string Name
        {
            get { return Resources.TelemetryTabName; }
            protected set { throw new NotImplementedException(); }
        }

        public bool Participate { get; set; }
    }
}
