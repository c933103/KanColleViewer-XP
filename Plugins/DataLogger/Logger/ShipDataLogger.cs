using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Logger
{
    public class ShipDataLogger
    {
        public ShipDataLogger()
        {
            DataStore.OnDataStoreCreate += (_, ds) => {
                ds.ShipDataChanged += procShipDataChanged;
            };
        }

        void procShipDataChanged(int shipId)
        {
        }
    }
}
