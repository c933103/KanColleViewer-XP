using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Logger
{
    static class ShipItemCreateLogger
    {
        static bool _initialized;

        public static void Init()
        {
            if (_initialized) return;
            _initialized = true;
            LynLoggerMain.OnInstanceCreate += i => {
                i.CreateItemObserver.OnItemCreate += item => {
                    DataStore.Store.Current.CurrentLogbook.ItemCreateLog.Append(item, 0);
                    DataStore.Store.Current.Weekbook.ItemCreateLog.Append(item, 0);
                };
                i.CreateShipObserver.OnShipCreate += ship => {
                    DataStore.Store.Current.CurrentLogbook.ShipCreateLog.Append(ship, 0);
                    DataStore.Store.Current.Weekbook.ShipCreateLog.Append(ship, 0);
                };
            };
        }
    }
}
