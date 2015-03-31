using LynLogger.DataStore.LogBook;
using LynLogger.Models;
using System.Collections.Generic;
using System.Linq;

namespace LynLogger.Views
{
    class ShipHistoryModel : NotificationSourceObject
    {
        private Ship _selected;
        public Ship SelectedShip {
            get { return _selected; }
            set
            {
                if(_selected == value) return;
                _selected = value;
                RaiseMultiPropertyChanged(() => SelectedShip, () => CombinedEventLog, () => SelectedShipExp);
            }
        }

        public IEnumerable<Ship> Ships
        {
            get
            {
                if(DataStore.Store.Current == null) return null;
                return new LinkedList<Ship>(DataStore.Store.Current.Weekbook.Ships);
            }
        }

        public IEnumerable<KeyValuePair<long, double>> SelectedShipExp
        {
            get
            {
                return SelectedShip?.Exp.Select(x => new KeyValuePair<long, double>(x.Key, x.Value));
            }
        }

        public IEnumerable<KeyValuePair<string, string>> CombinedEventLog
        {
            get
            {
                if(SelectedShip == null) return null;

                return new IEnumerable<KeyValuePair<long, string>>[] {
                    SelectedShip.ShipNameType.Select(x => new KeyValuePair<long, string>(x.Key, string.Format("新成员 {0} {1}", x.Value.TypeName, x.Value.ShipName))).Take(1),
                    SelectedShip.Level.Skip(1).Select(x => new KeyValuePair<long, string>(x.Key, string.Format("等级升到 {0} 级", x.Value))),
                    SelectedShip.ShipNameType.Select(x => new KeyValuePair<long, string>(x.Key, string.Format("改造为 {0} {1}", x.Value.TypeName, x.Value.ShipName))).Skip(1),
                    SelectedShip.EnhancedAntiAir.SelectWithPrevious((prev, curr) => new KeyValuePair<long, string>(curr.Key, "对空提升了" + (curr.Value - prev.Value))),
                    SelectedShip.EnhancedDefense.SelectWithPrevious((prev, curr) => new KeyValuePair<long, string>(curr.Key, "装甲提升了" + (curr.Value - prev.Value))),
                    SelectedShip.EnhancedPower.SelectWithPrevious((prev, curr) => new KeyValuePair<long, string>(curr.Key, "火力提升了" + (curr.Value - prev.Value))),
                    SelectedShip.EnhancedTorpedo.SelectWithPrevious((prev, curr) => new KeyValuePair<long, string>(curr.Key, "雷装提升了" + (curr.Value - prev.Value))),
                    SelectedShip.EnhancedLuck.SelectWithPrevious((prev, curr) => new KeyValuePair<long, string>(curr.Key, " 运 提升了" + (curr.Value - prev.Value))),
                    SelectedShip.SRate.Skip(1).Select(x => new KeyValuePair<long, string>(x.Key, string.Format("星级变为 {0} 星", x.Value+1))),
                    SelectedShip.ExistenceLog.Where(x => x.Value == ShipExistenceStatus.NonExistence).Select(x => new KeyValuePair<long, string>(x.Key, "除籍")),
                }.SelectMany(x => x).OrderBy(x => x.Key).Select(x => new KeyValuePair<string, string>(Helpers.FromUnixTimestamp(x.Key).LocalDateTime.ToString(), x.Value));
            }
        }

        public ShipHistoryModel()
        {
            DataStore.Store.OnDataStoreCreate += (_, store) => store.OnShipDataChange += (ds, x) => {
                if(x == SelectedShip?.Id) {
                    RaiseMultiPropertyChanged(() => SelectedShip, () => CombinedEventLog, () => SelectedShipExp);
                }
                RaiseMultiPropertyChanged(() => Ships);
            };
            DataStore.Store.OnDataStoreSwitch += (_, ds) => RaiseMultiPropertyChanged(() => Ships);
        }
    }
}
