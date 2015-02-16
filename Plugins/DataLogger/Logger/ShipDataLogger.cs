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
                store[shipId].ExistenceLog.Append(Models.ShipExistenceStatus.Existing);
            }
            var shipHisto = store[shipId];
            var ship = ds.Ships[shipId];
            if(ship == null) {
                shipHisto.ExistenceLog.Append(Models.ShipExistenceStatus.NonExistence);
                return;
            }
            shipHisto.EnhancedAntiAir[Helpers.UnixTimestamp / 3600] = ship.EnhancedAntiAir;
            shipHisto.EnhancedDefense[Helpers.UnixTimestamp / 3600] = ship.EnhancedDefense;
            shipHisto.EnhancedLuck[Helpers.UnixTimestamp / 3600] = ship.EnhancedLuck;
            shipHisto.EnhancedPower[Helpers.UnixTimestamp / 3600] = ship.EnhancedPower;
            shipHisto.EnhancedTorpedo[Helpers.UnixTimestamp / 3600] = ship.EnhancedTorpedo;
            shipHisto.Exp[Helpers.UnixTimestamp / 3600] = ship.Exp;
            shipHisto.Level[Helpers.UnixTimestamp / 3600] = ship.Level;
            shipHisto.SRate[Helpers.UnixTimestamp / 3600] = ship.SRate;

            shipHisto.ShipId.Append(ship.ShipId);
            shipHisto.TypeId.Append(ship.TypeId);

            if(ship.Locked) {
                shipHisto.ExistenceLog.Append(Models.ShipExistenceStatus.Locked);
            } else {
                shipHisto.ExistenceLog.Append(Models.ShipExistenceStatus.Existing);
            }
        }
    }
}
