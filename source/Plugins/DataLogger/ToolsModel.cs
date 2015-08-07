using LynLogger.Models;
using LynLogger.Views;
using LynLogger.Views.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LynLogger
{
    public class ToolsModel : NotificationSourceObject<ToolsModel>
    {
        public IList<TabViewItem> Pages { get; private set; }

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

        public ToolsModel()
        {
            var listPages = new List<TabViewItem>() {
                new TabViewItem("舰娘信息", () => new ShipStatusView()),
                new TabViewItem("历史记录", () => new HistoryView()),
                new TabViewItem("战斗速报", () => new BattleNetaView()),
                new TabViewItem("设置", () => new SettingsView()),
                new TabViewItem("关于", () => new AboutView())
            };
            listPages.ForEach(x => x.IsSelected = false);
            Pages = listPages;
            SelectedPage = Pages.First();
        }
    }
}
