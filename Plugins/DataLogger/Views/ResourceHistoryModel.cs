using LynLogger.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LynLogger.Views
{
    class ResourceHistoryModel : NotificationSourceObject
    {
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
                if(value == null) throw new ArgumentNullException();
                if(_page == value) return;
                if(!Pages.Contains(value)) throw new ArgumentException();

                if(_page != null) _page.IsSelected = false;
                _page = value;
                _page.IsSelected = true;
                RaisePropertyChanged();
            }
        }

        public ResourceHistoryModel()
        {
            var listPages = new List<TabViewItem>() {
                new HistogramTabViewItem("油", new Func<IEnumerable<KeyValuePair<long,double>>>(() => DataStore.Instance?.BasicInfoHistory.Fuel.Select(x => new KeyValuePair<long, double>(x.Key, x.Value)))),
                new HistogramTabViewItem("弹", new Func<IEnumerable<KeyValuePair<long,double>>>(() => DataStore.Instance?.BasicInfoHistory.Ammo.Select(x => new KeyValuePair<long, double>(x.Key, x.Value)))),
                new HistogramTabViewItem("钢", new Func<IEnumerable<KeyValuePair<long,double>>>(() => DataStore.Instance?.BasicInfoHistory.Steel.Select(x => new KeyValuePair<long, double>(x.Key, x.Value)))),
                new HistogramTabViewItem("铝", new Func<IEnumerable<KeyValuePair<long,double>>>(() => DataStore.Instance?.BasicInfoHistory.Bauxite.Select(x => new KeyValuePair<long, double>(x.Key, x.Value)))),
                new HistogramTabViewItem("桶", new Func<IEnumerable<KeyValuePair<long,double>>>(() => DataStore.Instance?.BasicInfoHistory.HsRepair.Select(x => new KeyValuePair<long, double>(x.Key, x.Value)))),
                new HistogramTabViewItem("喷火器", new Func<IEnumerable<KeyValuePair<long,double>>>(() => DataStore.Instance?.BasicInfoHistory.HsBuild.Select(x => new KeyValuePair<long, double>(x.Key, x.Value)))),
                new HistogramTabViewItem("开发", new Func<IEnumerable<KeyValuePair<long,double>>>(() => DataStore.Instance?.BasicInfoHistory.DevMaterial.Select(x => new KeyValuePair<long, double>(x.Key, x.Value)))),
                new HistogramTabViewItem("改修", new Func<IEnumerable<KeyValuePair<long,double>>>(() => DataStore.Instance?.BasicInfoHistory.ModMaterial.Select(x => new KeyValuePair<long, double>(x.Key, x.Value)))),
            };
            listPages.ForEach(x => {
                x.IsSelected = false;
                x.PropertyChanged += (s, e) => {
                    if((s as HistogramTabViewItem)?.IsSelected == true) {
                        _histogramFactory = (s as HistogramTabViewItem).Factory;
                        RaisePropertyChanged(() => ActiveHistogram);
                    }
                };
            });
            Pages = listPages;
            SelectedPage = Pages.First();

            DataStore.BasicInfoChanged += x => {
                RaisePropertyChanged(() => ActiveHistogram);
            };
            DataStore.OnDataStoreSwitch += (_, ds) => RaisePropertyChanged(() => ActiveHistogram);
        }

        public class HistogramTabViewItem : TabViewItem
        {
            public Func<IEnumerable<KeyValuePair<long, double>>> Factory { get; private set; }
            public HistogramTabViewItem(string name, Func<IEnumerable<KeyValuePair<long, double>>> factory) : base(name,null) { Factory = factory; }
        }
    }
}
