using LynLogger.DataStore;
using LynLogger.Models;
using LynLogger.Views.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Views
{
    class HistoryModel : NotificationSourceObject
    {
        protected override IReadOnlyDictionary<Expression<Func<object, object>>, List<Expression<Func<object, object>>>> PropertyDependency
        {
            get
            {
                return new Dictionary<Expression<Func<object, object>>, List<Expression<Func<object, object>>>> {
                    [o => ((HistoryModel)o).SelectedBook] = new List<Expression<Func<object, object>>> {
                        o => ((HistoryModel)o).Books,
                        o => ((HistoryModel)o).SelectedBookId }
                };
            }
        }

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

        private ulong _selectedBook;
        public IReadOnlyCollection<ulong> Books => Store.Current?.LogbookSequence;
        public ulong SelectedBookId
        {
            get { return _selectedBook; }
            set
            {
                if (value == _selectedBook) return;
                _selectedBook = value;
                RaisePropertyChanged();
            }
        }

        public ILogbook SelectedBook
        {
            get
            {
                return Store.Current?.Logbooks[SelectedBookId];
            }
        }

        public HistoryModel()
        {
            var listPages = new List<TabViewItem>() {
                new TabViewItem("建造", () => new History.ShipCreateHistoryView() { ShipCreateLogBinding = new System.Windows.Data.Binding(nameof(SelectedBook)+"."+nameof(SelectedBook.ShipCreateLog)) {Source = this } }),
                new TabViewItem("开发", () => new History.ItemCreateHistoryView() { ItemCreateLogBinding = new System.Windows.Data.Binding(nameof(SelectedBook)+"."+nameof(SelectedBook.ItemCreateLog)) {Source = this } }),
                new TabViewItem("资源", () => new History.ResourceHistoryView() { BasicInfoLogBinding = new System.Windows.Data.Binding(nameof(SelectedBook)+"."+nameof(SelectedBook.BasicInfo)) {Source = this } }),
                new TabViewItem("出击", () => new History.SortieHistoryView() { SortieLogBinding = new System.Windows.Data.Binding(nameof(SelectedBook)+"."+nameof(SelectedBook.SortieLog)) {Source = this } }),
                new TabViewItem("演习", () => new History.DrillHistoryView() { DrillLogBinding = new System.Windows.Data.Binding(nameof(SelectedBook)+"."+nameof(SelectedBook.DrillLog)) {Source = this } }),
                new TabViewItem("舰娘", () => new History.ShipHistoryView() { ShipHistoryBinding = new System.Windows.Data.Binding(nameof(SelectedBook)+"."+nameof(SelectedBook.Ships)) {Source = this } }),
            };
            
            listPages.ForEach(x => x.IsSelected = false);
            Pages = listPages;
            SelectedPage = Pages.First();

            Store.OnDataStoreSwitch += _ => {
                RaiseMultiPropertyChanged(() => Books);
                SelectedBookId = 0;
            };
        }
    }
}
