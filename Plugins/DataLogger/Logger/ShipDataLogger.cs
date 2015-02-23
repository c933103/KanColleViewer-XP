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
                ds.ShipDataChanged += x => ProcShipDataChanged(ds, x);
            };
        }

        void ProcShipDataChanged(DataStore ds, int shipId)
        {
            var store = ds.i_ShipHistories;
            if(!store.ContainsKey(shipId)) {
                store[shipId] = new Models.ShipHistory(shipId);
                store[shipId].ExistenceLog.Append(Models.ShipExistenceStatus.Existing, 0);
            }
            var shipHisto = store[shipId];
            var ship = ds.Ships[shipId];
            if(ship == null) {
                shipHisto.ExistenceLog.Append(Models.ShipExistenceStatus.NonExistence, 0);
                return;
            }
            shipHisto.EnhancedAntiAir.Append(ship.EnhancedAntiAir);
            shipHisto.EnhancedDefense.Append(ship.EnhancedDefense);
            shipHisto.EnhancedLuck.Append(ship.EnhancedLuck);
            shipHisto.EnhancedPower.Append(ship.EnhancedPower);
            shipHisto.EnhancedTorpedo.Append(ship.EnhancedTorpedo);
            shipHisto.Exp.Append(ship.Exp);
            shipHisto.Level.Append(ship.Level);
            shipHisto.SRate.Append(ship.SRate);

            shipHisto.ShipId.Append(ship.ShipId, 0);
            shipHisto.TypeId.Append(ship.TypeId, 0);

            if(ship.Locked) {
                shipHisto.ExistenceLog.Append(Models.ShipExistenceStatus.Locked, 0);
            } else {
                shipHisto.ExistenceLog.Append(Models.ShipExistenceStatus.Existing, 0);
            }
        }
    }
}
