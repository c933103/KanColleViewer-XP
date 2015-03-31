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
                new HistogramTabViewItem("油", new Func<IEnumerable<KeyValuePair<long,double>>>(() => DataStore.Store.Current?.Weekbook.BasicInfo.Fuel.Select(x => new KeyValuePair<long, double>(x.Key, x.Value)))),
                new HistogramTabViewItem("弹", new Func<IEnumerable<KeyValuePair<long,double>>>(() => DataStore.Store.Current?.Weekbook.BasicInfo.Ammo.Select(x => new KeyValuePair<long, double>(x.Key, x.Value)))),
                new HistogramTabViewItem("钢", new Func<IEnumerable<KeyValuePair<long,double>>>(() => DataStore.Store.Current?.Weekbook.BasicInfo.Steel.Select(x => new KeyValuePair<long, double>(x.Key, x.Value)))),
                new HistogramTabViewItem("铝", new Func<IEnumerable<KeyValuePair<long,double>>>(() => DataStore.Store.Current?.Weekbook.BasicInfo.Bauxite.Select(x => new KeyValuePair<long, double>(x.Key, x.Value)))),
                new HistogramTabViewItem("桶", new Func<IEnumerable<KeyValuePair<long,double>>>(() => DataStore.Store.Current?.Weekbook.BasicInfo.HsRepair.Select(x => new KeyValuePair<long, double>(x.Key, x.Value)))),
                new HistogramTabViewItem("喷火器", new Func<IEnumerable<KeyValuePair<long,double>>>(() => DataStore.Store.Current?.Weekbook.BasicInfo.HsBuild.Select(x => new KeyValuePair<long, double>(x.Key, x.Value)))),
                new HistogramTabViewItem("开发", new Func<IEnumerable<KeyValuePair<long,double>>>(() => DataStore.Store.Current?.Weekbook.BasicInfo.DevMaterial.Select(x => new KeyValuePair<long, double>(x.Key, x.Value)))),
                new HistogramTabViewItem("改修", new Func<IEnumerable<KeyValuePair<long,double>>>(() => DataStore.Store.Current?.Weekbook.BasicInfo.ModMaterial.Select(x => new KeyValuePair<long, double>(x.Key, x.Value)))),
            };
            listPages.ForEach(x => {
                x.IsSelected = false;
                x.PropertyChanged += (s, e) => {
                    if((s as HistogramTabViewItem)?.IsSelected == true) {
                        _histogramFactory = (s as HistogramTabViewItem).Factory;
                        RaiseMultiPropertyChanged(() => ActiveHistogram);
                    }
                };
            });
            Pages = listPages;
            SelectedPage = Pages.First();

            DataStore.Store.OnDataStoreCreate += (_, ds) => ds.OnBasicInfoChange += x => {
                RaiseMultiPropertyChanged(() => ActiveHistogram);
            };
            DataStore.Store.OnDataStoreSwitch += (_, ds) => RaiseMultiPropertyChanged(() => ActiveHistogram);
        }

        public class HistogramTabViewItem : TabViewItem
        {
            public Func<IEnumerable<KeyValuePair<long, double>>> Factory { get; private set; }
            public HistogramTabViewItem(string name, Func<IEnumerable<KeyValuePair<long, double>>> factory) : base(name,null) { Factory = factory; }
        }
    }
}
