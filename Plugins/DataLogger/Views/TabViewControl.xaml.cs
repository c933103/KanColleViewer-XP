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

namespace LynLogger.Views
{
    /// <summary>
    /// TabViewControl.xaml 的交互逻辑
    /// </summary>
    public partial class TabViewControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public TabViewControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty dpTabViewItems = DependencyProperty.Register("TabViewItems", typeof(IList<TabViewItem>), typeof(TabViewControl));
        public static readonly DependencyProperty dpTabViewSelected = DependencyProperty.Register("TabViewSelected", typeof(TabViewItem), typeof(TabViewControl));

        public IList<TabViewItem> TabViewItems
        {
            get { return (IList<TabViewItem>)GetValue(dpTabViewItems); }
            set
            {
                SetValue(dpTabViewItems, value);
                if(PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("TabViewItems"));
            }
        }

        public TabViewItem TabViewSelected
        {
            get { return (TabViewItem)GetValue(dpTabViewSelected); }
            set
            {
                SetValue(dpTabViewSelected, value);
                if(PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("TabViewSelected"));
            }
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TabViewSelected = (TabViewItem)((Grid)sender).DataContext;
        }
    }

    public class TabViewItem : Models.NotificationSourceObject
    {
        public string TabName { get; set; }
        public object TabView { get; set; }

        private bool _selected = false;
        public bool IsSelected
        {
            get { return _selected; }
            set
            {
                if(_selected == value) return;
                _selected = value;
                RaisePropertyChanged();
            }
        }

        public TabViewItem(string name, object view)
        {
            TabName = name;
            TabView = view;
        }
    }
}
