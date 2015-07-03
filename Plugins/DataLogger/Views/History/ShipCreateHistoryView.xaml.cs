using LynLogger.DataStore.LogBook;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace LynLogger.Views.History
{
    /// <summary>
    /// ShipCreateHistoryView.xaml 的交互逻辑
    /// </summary>
    public partial class ShipCreateHistoryView : UserControl, INotifyPropertyChanged
    {
        public ShipCreateHistoryView()
        {
            InitializeComponent();
            LynLoggerMain.OnInstanceCreate += i =>  i.CreateShipObserver.OnShipCreate += _ => RaisePropertyChanged();
        }

        public static DependencyProperty dpShipCreateLog = DependencyProperty.Register(nameof(ShipCreateLog), typeof(Histogram<ShipCreate>), typeof(ShipCreateHistoryView),
                                                                    new PropertyMetadata((o, e) => ((ShipCreateHistoryView)o).RaisePropertyChanged()));

        public Histogram<ShipCreate> ShipCreateLog
        {
            get { return (Histogram<ShipCreate>)GetValue(dpShipCreateLog); }
            set { SetValue(dpShipCreateLog, value); }
        }

        //Turns out we have to do this, as the collection doesn't implement INotifyCollectionChange
        public IEnumerable<KeyValuePair<long, ShipCreate>> ShipCreateLog2 => ShipCreateLog.Select(x => x);

        public BindingBase ShipCreateLogBinding
        {
            set { SetBinding(dpShipCreateLog, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged()
        {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(nameof(ShipCreateLog)));
                handler(this, new PropertyChangedEventArgs(nameof(ShipCreateLog2)));
            }
        }
    }
}
