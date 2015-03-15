using LynLogger.Models;
using LynLogger.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LynLogger
{
    public class ToolsModel : NotificationSourceObject
    {
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

        public ToolsModel()
        {
            var listPages = new List<TabViewItem>() {
                new TabViewItem("舰娘信息", new ShipStatusView()/*{DataContext=new ResourceHistoryModel()}*/),
                new TabViewItem("资源历史", new ResourceHistoryView()/*{DataContext=new ResourceHistoryModel()}*/),
                new TabViewItem("档案室", new ShipHistoryView()/*{DataContext=new ShipHistoryModel()}*/),
                new TabViewItem("战斗剧透", new BattleNetaView()/*{DataContext = new BattleNetaModel()}*/),
                new TabViewItem("肝度", new GradeEvaluationView()/*{DataContext=new GradeEvaluationModel()}*/),
                new TabViewItem("设置", new Settings.SettingsView()/*{DataContext = new Settings.SettingsModel()}*/),
                new TabViewItem("关于", new Settings.AboutView())
            };
            listPages.ForEach(x => x.IsSelected = false);
            Pages = listPages;
            SelectedPage = Pages.First();
        }
    }
}
