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

namespace LynLogger.Views.BattleLog
{
    /// <summary>
    /// DrillHistoryView.xaml 的交互逻辑
    /// </summary>
    public partial class DrillHistoryView : UserControl, INotifyPropertyChanged
    {
        public DrillHistoryView()
        {
            InitializeComponent();
            Logger.DrillLogger.OnNewLogEntry += () => RaisePropertyChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static DependencyProperty dpDrillLog = DependencyProperty.Register(nameof(DrillLog), typeof(Histogram<DrillInfo>), typeof(DrillHistoryView),
                                                                    new PropertyMetadata((o, e) => ((DrillHistoryView)o).RaisePropertyChanged()));

        public Histogram<DrillInfo> DrillLog
        {
            get { return (Histogram<DrillInfo>)GetValue(dpDrillLog); }
            set { SetValue(dpDrillLog, value); }
        }

        public IEnumerable<KeyValuePair<long, DrillInfo>> DrillLog2 => DrillLog?.Select(x => x);

        public BindingBase DrillLogBinding
        {
            set { SetBinding(dpDrillLog, value); }
        }

        private void RaisePropertyChanged()
        {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(nameof(DrillLog)));
                handler(this, new PropertyChangedEventArgs(nameof(DrillLog2)));
            }
        }
    }
}
