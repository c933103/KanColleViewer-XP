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
    /// ItemCreateHistoryView.xaml 的交互逻辑
    /// </summary>
    public partial class ItemCreateHistoryView : UserControl, INotifyPropertyChanged
    {
        public ItemCreateHistoryView()
        {
            InitializeComponent();
            LynLoggerMain.OnInstanceCreate += i => i.CreateItemObserver.OnItemCreate += _ => RaisePropertyChanged();
        }

        public static DependencyProperty dpItemCreateLog = DependencyProperty.Register(nameof(ItemCreateLog), typeof(Histogram<ItemCreate>), typeof(ItemCreateHistoryView),
                                                                    new PropertyMetadata((o, e) => ((ItemCreateHistoryView)o).RaisePropertyChanged()));

        public Histogram<ItemCreate> ItemCreateLog
        {
            get { return (Histogram<ItemCreate>)GetValue(dpItemCreateLog); }
            set { SetValue(dpItemCreateLog, value); }
        }

        //Turns out we have to do this, as the collection doesn't implement INotifyCollectionChange
        public IEnumerable<KeyValuePair<long, ItemCreate>> ItemCreateLog2 => ItemCreateLog?.Select(x => x);

        public BindingBase ItemCreateLogBinding
        {
            set { SetBinding(dpItemCreateLog, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged()
        {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(nameof(ItemCreateLog)));
                handler(this, new PropertyChangedEventArgs(nameof(ItemCreateLog2)));
            }
        }
    }
}
