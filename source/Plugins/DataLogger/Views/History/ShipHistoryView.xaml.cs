using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LynLogger.DataStore.LogBook;
using System.ComponentModel;
using LynLogger.DataStore;
using LynLogger.Utilities;

namespace LynLogger.Views.History
{
    /// <summary>
    /// ShipHistoryView.xaml 的交互逻辑
    /// </summary>
    public partial class ShipHistoryView : UserControl, INotifyPropertyChanged
    {
        public ShipHistoryView()
        {
            InitializeComponent();

            Store.OnDataStoreCreate += store => store.OnShipDataChange += (ds, x) => RaiseShipDataChanged(x);
            Store.OnDataStoreSwitch += _ => RaiseShipListChanged();
        }

        public static DependencyProperty dpShipHistory = DependencyProperty.Register(nameof(ShipHistory), typeof(IShipsLogAccessor), typeof(ShipHistoryView),
                                                                    new PropertyMetadata((o, e) => ((ShipHistoryView)o).RaiseShipListChanged()));

        public IShipsLogAccessor ShipHistory
        {
            get { return (IShipsLogAccessor)GetValue(dpShipHistory); }
            set { SetValue(dpShipHistory, value); }
        }

        public BindingBase ShipHistoryBinding
        {
            set { SetBinding(dpShipHistory, value); }
        }

        private Ship _selected;
        public Ship SelectedShip
        {
            get { return _selected; }
            set
            {
                if (_selected == value) return;
                _selected = value;
                RaiseShipChanged();
            }
        }

        public IEnumerable<Ship> Ships
        {
            get
            {
                return ShipHistory == null ? null : new LinkedList<Ship>(ShipHistory);
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
                if (SelectedShip == null) return null;

                return new IEnumerable<KeyValuePair<long, string>>[] {
                    SelectedShip.ShipNameType.Select(x => new KeyValuePair<long, string>(x.Key, string.Format("{0} {1} 记录中", x.Value.TypeName, x.Value.ShipName))).Take(1),
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaiseShipChanged()
        {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(nameof(CombinedEventLog)));
                handler(this, new PropertyChangedEventArgs(nameof(SelectedShipExp)));
                handler(this, new PropertyChangedEventArgs(nameof(SelectedShip)));
            }
        }

        private void RaiseShipListChanged()
        {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(nameof(ShipHistory)));
                handler(this, new PropertyChangedEventArgs(nameof(Ships)));
            }
            SelectedShip = null;
        }

        private void RaiseShipDataChanged(int shipId)
        {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(nameof(Ships)));
            }
            if (shipId == SelectedShip?.Id) {
                RaiseShipChanged();
            }
        }

        public ICommand CmdDeleteSelected => CommandDeleteSelected.Instance;
        public ICommand CmdCleanNonExsistent => CommandCleanNonExsistent.Instance;

        private class CommandDeleteSelected : ICommand
        {
            public static ICommand Instance = new CommandDeleteSelected();

            public event EventHandler CanExecuteChanged { add { } remove { } }

            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter)
            {
                if (parameter != null && parameter is ShipHistoryView) {
                    var par = (ShipHistoryView)parameter;
                    if (par.ShipHistory == null) return;
                    if (par.SelectedShip == null) return;
                    par.ShipHistory.Remove(par.SelectedShip.Id);
                    par.RaiseShipListChanged();
                }
            }
        }

        private class CommandCleanNonExsistent : ICommand
        {
            public static ICommand Instance = new CommandCleanNonExsistent();

            public event EventHandler CanExecuteChanged { add { } remove { } }

            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter)
            {
                if (parameter != null && parameter is ShipHistoryView) {
                    var par = (ShipHistoryView)parameter;
                    if (par.ShipHistory == null) return;
                    var ids = par.ShipHistory.Where(x => 
                        x.ExistenceLog.Last().Value == ShipExistenceStatus.NonExistence
                        || (
                            x.EnhancedAntiAir.Count <= 1
                         && x.EnhancedDefense.Count <= 1
                         && x.EnhancedLuck.Count <= 1
                         && x.EnhancedPower.Count <= 1
                         && x.EnhancedTorpedo.Count <= 1
                         && x.ExistenceLog.Count <= 1
                         && x.Exp.Count <= 1
                         && x.Level.Count <= 1
                         && x.ShipNameType.Count <= 1
                         && x.SRate.Count <= 1
                        )
                    ).Select(x => x.Id).ToList();
                    foreach(var id in ids) {
                        par.ShipHistory.Remove(id);
                    }
                    par.RaiseShipListChanged();
                }
            }
        }
    }
}
