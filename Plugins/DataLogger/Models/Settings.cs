using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Models
{
    [Serializable]
    public class Settings
    {
        public int BasicInfoLoggingInterval { get; internal set; }
        public int ShipDataLoggingInterval { get; internal set; }

        public Settings()
        {
            BasicInfoLoggingInterval = 7200;
            ShipDataLoggingInterval = 1800;
        }
    }
}
