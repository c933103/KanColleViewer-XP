using Grabacr07.KanColleWrapper.Models.Raw;
using LynLogger.Models;
using System.Linq;

namespace LynLogger.Observers
{
    static class ShipUpdater
    {
        public static void UpdateShips(this DataStore dsRoot, kcsapi_ship2[] raw)
        {
            var ds = dsRoot.RwShips;

            var shipIds = ds.Keys.ToList();
            foreach(var ship in raw) {
                if(!ds.ContainsKey(ship.api_id)) {
                    ds[ship.api_id] = new Ship(ship.api_id, ship.api_ship_id);
                }
                ds[ship.api_id].Update(ship);
                shipIds.Remove(ship.api_id);
            }

            //Remove ships that no longer exists from ds.
            foreach(var id in shipIds) {
                ds[id] = null;
                Models.DataStore.Instance.RaiseShipDataChange(id);
                ds.Remove(id);
            }
        }
    }
}
