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
    class BattleLogModel : NotificationSourceObject
    {
        protected override IReadOnlyDictionary<Expression<Func<object, object>>, List<Expression<Func<object, object>>>> PropertyDependency
        {
            get
            {
                return new Dictionary<Expression<Func<object, object>>, List<Expression<Func<object, object>>>> {
                    [o => ((BattleLogModel)o).SelectedBook] = new List<Expression<Func<object, object>>> {
                        o => ((BattleLogModel)o).Books,
                        o => ((BattleLogModel)o).SelectedBookId }
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

        public BattleLogModel()
        {
            var listPages = new List<TabViewItem>() {
                new TabViewItem("出击记录", () => new BattleLog.SortieHistoryView() { SortieLogBinding = new System.Windows.Data.Binding(nameof(SelectedBook)+"."+nameof(SelectedBook.SortieLog)) {Source = this } }),
                new TabViewItem("演习记录", () => new BattleLog.DrillHistoryView() { DrillLogBinding = new System.Windows.Data.Binding(nameof(SelectedBook)+"."+nameof(SelectedBook.DrillLog)) {Source = this } }),
                //new TabViewItem("统计数据", new ShipHistoryView()),
            };
            
            listPages.ForEach(x => x.IsSelected = false);
            Pages = listPages;
            SelectedPage = Pages.First();

            Store.OnDataStoreSwitch += (_, ds) => {
                RaiseMultiPropertyChanged(() => Books);
                SelectedBookId = 0;
            };
        }
    }
}
