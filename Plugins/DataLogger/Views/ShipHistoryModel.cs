using LynLogger.Models;
using System.Collections.Generic;
using System.Linq;

namespace LynLogger.Views
{
    class ShipHistoryModel : NotificationSourceObject
    {
        private ShipHistory _selected;
        public ShipHistory SelectedShip {
            get { return _selected; }
            set
            {
                if(_selected == value) return;
                _selected = value;
                RaisePropertyChanged(o => SelectedShip, o => CombinedEventLog, o => SelectedShipExp);
            }
        }

        public IEnumerable<ShipHistory> Ships
        {
            get
            {
                if(DataStore.Instance == null) return null;
                return DataStore.Instance.ShipHistories.Select(x => x.Value);
            }
        }

        public IEnumerable<KeyValuePair<long, double>> SelectedShipExp
        {
            get
            {
                if(SelectedShip == null) return null;
                return SelectedShip.Exp.Select(x => new KeyValuePair<long, double>(x.Key, x.Value));
            }
        }

        public IEnumerable<KeyValuePair<string, string>> CombinedEventLog
        {
            get
            {
                if(SelectedShip == null) return null;

                return new IEnumerable<KeyValuePair<long, string>>[] {
                    Enumerable.Zip(SelectedShip.TypeName, SelectedShip.ShipName, (a, b) => new KeyValuePair<long, string>(a.Key, string.Format("新成员 {0} {1}", a.Value, b.Value))).Take(1),
                    SelectedShip.Level.Skip(1).Select(x => new KeyValuePair<long, string>(x.Key, string.Format("等级升到 {0} 级", x.Value))),
                    Enumerable.Zip(SelectedShip.TypeName, SelectedShip.ShipName, (a, b) => new KeyValuePair<long, string>(a.Key, string.Format("改造为 {0} {1}", a.Value, b.Value))).Skip(1),
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
            DataStore.ShipDataChanged += (ds, x) => {
                if(SelectedShip != null && x == SelectedShip.Id) {
                    RaisePropertyChanged(o => SelectedShip, o => CombinedEventLog, o => SelectedShipExp);
                }
                RaisePropertyChanged(o => Ships);
            };
            DataStore.OnDataStoreSwitch += (_, ds) => RaisePropertyChanged(o => Ships);
        }
    }
}
