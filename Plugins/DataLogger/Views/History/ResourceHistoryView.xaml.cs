using LynLogger.Views.Controls;
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
    /// ResourceHistoryView.xaml 的交互逻辑
    /// </summary>
    public partial class ResourceHistoryView : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static DependencyProperty dpBasicInfoLog = DependencyProperty.Register(nameof(BasicInfoLog), typeof(DataStore.LogBook.BasicInfo), typeof(ResourceHistoryView),
                                                                    new PropertyMetadata((o, e) => ((ResourceHistoryView)o).RaisePropertyChanged()));

        public DataStore.LogBook.BasicInfo BasicInfoLog
        {
            get { return (DataStore.LogBook.BasicInfo)GetValue(dpBasicInfoLog); }
            set { SetValue(dpBasicInfoLog, value); }
        }

        public BindingBase BasicInfoLogBinding
        {
            set { SetBinding(dpBasicInfoLog, value); }
        }

        public ResourceHistoryView()
        {
            var listPages = new List<TabViewItem>() {
                new HistogramTabViewItem("油", new Func<IEnumerable<KeyValuePair<long,double>>>(() => BasicInfoLog?.Fuel.Select(x => new KeyValuePair<long, double>(x.Key, x.Value)))),
                new HistogramTabViewItem("弹", new Func<IEnumerable<KeyValuePair<long,double>>>(() => BasicInfoLog?.Ammo.Select(x => new KeyValuePair<long, double>(x.Key, x.Value)))),
                new HistogramTabViewItem("钢", new Func<IEnumerable<KeyValuePair<long,double>>>(() => BasicInfoLog?.Steel.Select(x => new KeyValuePair<long, double>(x.Key, x.Value)))),
                new HistogramTabViewItem("铝", new Func<IEnumerable<KeyValuePair<long,double>>>(() => BasicInfoLog?.Bauxite.Select(x => new KeyValuePair<long, double>(x.Key, x.Value)))),
                new HistogramTabViewItem("桶", new Func<IEnumerable<KeyValuePair<long,double>>>(() => BasicInfoLog?.HsRepair.Select(x => new KeyValuePair<long, double>(x.Key, x.Value)))),
                new HistogramTabViewItem("喷火器", new Func<IEnumerable<KeyValuePair<long,double>>>(() => BasicInfoLog?.HsBuild.Select(x => new KeyValuePair<long, double>(x.Key, x.Value)))),
                new HistogramTabViewItem("开发", new Func<IEnumerable<KeyValuePair<long,double>>>(() => BasicInfoLog?.DevMaterial.Select(x => new KeyValuePair<long, double>(x.Key, x.Value)))),
                new HistogramTabViewItem("改修", new Func<IEnumerable<KeyValuePair<long,double>>>(() => BasicInfoLog?.ModMaterial.Select(x => new KeyValuePair<long, double>(x.Key, x.Value)))),
            };
            listPages.ForEach(x => {
                x.IsSelected = false;
                x.PropertyChanged += (s, e) => {
                    if ((s as HistogramTabViewItem)?.IsSelected == true) {
                        _histogramFactory = (s as HistogramTabViewItem).Factory;
                    }
                };
            });
            Pages = listPages;
            SelectedPage = Pages.First();
            DataStore.Store.OnDataStoreCreate += (_, ds) => ds.OnBasicInfoChange += x => {
                RaisePropertyChanged();
            };

            InitializeComponent();
        }

        public IEnumerable<KeyValuePair<long, double>> ActiveHistogram
        {
            get { return _histogramFactory == null ? null : _histogramFactory(); }
        }

        private Func<IEnumerable<KeyValuePair<long, double>>> _histogramFactory;

        public IReadOnlyList<TabViewItem> Pages { get; private set; }

        private TabViewItem _page;
        public TabViewItem SelectedPage
        {
            get { return _page; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                if (_page == value) return;
                if (!Pages.Contains(value)) throw new ArgumentException();

                if (_page != null) _page.IsSelected = false;
                _page = value;
                _page.IsSelected = true;
                RaisePropertyChanged();
            }
        }

        public class HistogramTabViewItem : TabViewItem
        {
            public Func<IEnumerable<KeyValuePair<long, double>>> Factory { get; private set; }
            public HistogramTabViewItem(string name, Func<IEnumerable<KeyValuePair<long, double>>> factory) : base(name, null) { Factory = factory; }
        }

        private void RaisePropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActiveHistogram)));
        }
    }
}
