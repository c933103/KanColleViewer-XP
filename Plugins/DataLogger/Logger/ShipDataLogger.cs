namespace LynLogger.Logger
{
    public class ShipDataLogger
    {
        public ShipDataLogger()
        {
            Models.DataStore.ShipDataChanged += (sender, x) => ProcShipDataChanged(sender, x);
        }

        void ProcShipDataChanged(Models.DataStore ds, int shipId)
        {
            var store = ds.RwShipHistories;
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
            shipHisto.EnhancedAntiAir.Append(ship.EnhancedAntiAir, ds.Settings.ShipDataLoggingInterval);
            shipHisto.EnhancedDefense.Append(ship.EnhancedDefense, ds.Settings.ShipDataLoggingInterval);
            shipHisto.EnhancedLuck.Append(ship.EnhancedLuck, ds.Settings.ShipDataLoggingInterval);
            shipHisto.EnhancedPower.Append(ship.EnhancedPower, ds.Settings.ShipDataLoggingInterval);
            shipHisto.EnhancedTorpedo.Append(ship.EnhancedTorpedo, ds.Settings.ShipDataLoggingInterval);
            shipHisto.Exp.Append(ship.Exp, ds.Settings.ShipDataLoggingInterval);
            shipHisto.Level.Append(ship.Level, ds.Settings.ShipDataLoggingInterval);
            shipHisto.SRate.Append(ship.SRate, ds.Settings.ShipDataLoggingInterval);
            shipHisto.ShipNameType.Append(Helpers.LookupShipNameInfo(ship.ShipId), 0);

            if(ship.Locked) {
                shipHisto.ExistenceLog.Append(Models.ShipExistenceStatus.Locked, 0);
            } else {
                shipHisto.ExistenceLog.Append(Models.ShipExistenceStatus.Existing, 0);
            }
        }
    }
}
