using Grabacr07.KanColleWrapper.Models.Raw;
using LynLogger.DataStore;
using LynLogger.Models;
using System.Linq;

namespace LynLogger.Observers
{
    static class ShipUpdater
    {
        public static void UpdateShips(this Store dsRoot, kcsapi_ship2[] raw)
        {
            var ds = dsRoot.Ships;
            var shipIds = ds.AllIds.ToList();
            foreach(var ship in raw) {
                ds[ship.api_id].Update(ship);
                shipIds.Remove(ship.api_id);
            }

            //Remove ships that no longer exists from ds.
            foreach(var id in shipIds) {
                ds[id] = null;
                dsRoot.RaiseShipDataChange(id);
            }
        }
    }
}
