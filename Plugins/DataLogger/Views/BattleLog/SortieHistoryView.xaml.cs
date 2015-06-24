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
    /// SortieHistoryView.xaml 的交互逻辑
    /// </summary>
    public partial class SortieHistoryView : UserControl, INotifyPropertyChanged
    {
        public SortieHistoryView()
        {
            InitializeComponent();
            Logger.SortieLogger.OnNewLogEntry += () => RaisePropertyChanged();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static DependencyProperty dpSortieLog = DependencyProperty.Register(nameof(SortieLog), typeof(Histogram<SortieInfo>), typeof(SortieHistoryView),
                                                                    new PropertyMetadata((o, e) => ((SortieHistoryView)o).RaisePropertyChanged()));

        public Histogram<SortieInfo> SortieLog
        {
            get { return (Histogram<SortieInfo>)GetValue(dpSortieLog); }
            set { SetValue(dpSortieLog, value); }
        }

        public BindingBase SortieLogBinding
        {
            set { SetBinding(dpSortieLog, value); }
        }

        private SortieInfo _sortie;
        public SortieInfo Sortie
        {
            get { return _sortie; }
            set
            {
                _sortie = value; Node = value?.Nodes.First();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Sortie)));
            }
        }

        public KeyValuePair<long, SortieInfo> KvSortie { set { Sortie = value.Value; } }

        private SortieInfo.Node _node;
        public SortieInfo.Node Node
        {
            get { return _node; }
            set
            {
                _node = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Node)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasBattle)));
            }
        }

        public bool HasBattle => _node?.Battle != null;

        private void RaisePropertyChanged()
        {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(nameof(SortieLog)));
            }
        }
    }
}
