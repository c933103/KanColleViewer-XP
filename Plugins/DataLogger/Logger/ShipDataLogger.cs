using LynLogger.DataStore;
using LynLogger.DataStore.LogBook;

namespace LynLogger.Logger
{
    static class ShipDataLogger
    {
        static bool _initialized;

        public static void Init()
        {
            if (_initialized) return;
            _initialized = true;
            Store.OnDataStoreCreate += (sid, store) => store.OnShipDataChange += (sender, x) => ProcShipDataChanged(sender, x);
        }

        static void ProcShipDataChanged(Store ds, int shipId)
        {
            var ship = ds.Ships[shipId];
            var histo = new IShipsLogAccessor[] { ds.CurrentLogbook.Ships, ds.Weekbook.Ships };
            foreach(var storage in histo) {
                Ship shipHisto;
                if(ship == null) {
                    if(!storage.Contains(shipId)) continue;
                    shipHisto = storage[shipId];
                    shipHisto.ExistenceLog.Append(ShipExistenceStatus.NonExistence, 0);
                } else {
                    shipHisto = storage[shipId];
                    shipHisto.EnhancedAntiAir.Append(ship.EnhancedAntiAir, ds.Settings.ShipDataLoggingInterval, false);
                    shipHisto.EnhancedDefense.Append(ship.EnhancedDefense, ds.Settings.ShipDataLoggingInterval, false);
                    shipHisto.EnhancedLuck.Append(ship.EnhancedLuck, ds.Settings.ShipDataLoggingInterval, false);
                    shipHisto.EnhancedPower.Append(ship.EnhancedPower, ds.Settings.ShipDataLoggingInterval, false);
                    shipHisto.EnhancedTorpedo.Append(ship.EnhancedTorpedo, ds.Settings.ShipDataLoggingInterval, false);
                    shipHisto.Exp.Append(ship.Exp, ds.Settings.ShipDataLoggingInterval);
                    shipHisto.Level.Append(ship.Level, ds.Settings.ShipDataLoggingInterval, false);
                    shipHisto.SRate.Append(ship.SRate, ds.Settings.ShipDataLoggingInterval, false);
                    shipHisto.ShipNameType.Append(ship.ShipInfo, 0, false);

                    if(ship.Locked) {
                        shipHisto.ExistenceLog.Append(ShipExistenceStatus.Locked, 0, false);
                    } else {
                        shipHisto.ExistenceLog.Append(ShipExistenceStatus.Existing, 0, false);
                    }
                }

                shipHisto.RefreshUpdateTime();
            }
        }
    }
}
