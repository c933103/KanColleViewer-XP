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

        public DataStore CurrentActiveDs
        {
            get { return DataStore.Instance; }
        }

        public ToolsModel()
        {
            var listPages = new List<TabViewItem>() {
                new TabViewItem("肝度", new Views.GradeEvaluationView(){DataContext=new Views.GradeEvaluationModel()}),
                new TabViewItem("资源历史", new Views.ResourceHistoryView(){DataContext=new Views.ResourceHistoryModel()}),
                new TabViewItem("舰娘档案", new Views.ShipHistoryView(){DataContext=new Views.ShipHistoryModel()}),
                new TabViewItem("战斗剧透", new Views.BattleNetaView(){DataContext = new Views.BattleNetaModel()}),
                new TabViewItem("设置", new Settings.SettingsView(){DataContext = new Settings.SettingsModel()}),
                new TabViewItem("关于", new Settings.AboutView())
            };
            listPages.ForEach(x => x.IsSelected = false);
            Pages = listPages;
            SelectedPage = Pages.First();

            DataStore.OnDataStoreSwitch += (_, ds) => {
                this.RaisePropertyChanged(o => CurrentActiveDs);
            };
        }
    }
}
