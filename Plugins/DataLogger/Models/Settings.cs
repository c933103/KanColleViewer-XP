using System;

namespace LynLogger.Models
{
    [Serializable]
    public class Settings
    {
        public int BasicInfoLoggingInterval { get; internal set; }
        public int ShipDataLoggingInterval { get; internal set; }

        public Settings()
        {
            BasicInfoLoggingInterval = 14400;
            ShipDataLoggingInterval = 7200;
        }
    }
}
