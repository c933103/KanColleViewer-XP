using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LynLogger.DataStore.Serialization;

namespace LynLogger.DataStore.MasterInfo
{
    public class Settings : AbstractDSSerializable<Settings>
    {
        [Serialize(0)] public int BasicInfoLoggingInterval { get; set; }
        [Serialize(1)] public int ShipDataLoggingInterval { get; set; }

        public Settings()
        {
            BasicInfoLoggingInterval = 21600;
            ShipDataLoggingInterval = 10800;
        }

        public Settings(Premitives.StoragePremitive info, LinkedList<object> serializationPath) : base(info, serializationPath) { }
    }
}
