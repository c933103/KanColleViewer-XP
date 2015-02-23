using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LynLogger.Models;
using LynLogger.Views;

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
                RaisePropertyChanged("SelectedPage");
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
                new TabViewItem("关于", new Settings.AboutView())
            };
            listPages.ForEach(x => x.IsSelected = false);
            Pages = listPages;
            SelectedPage = Pages.First();

            DataStore.OnDataStoreCreate += (_, ds) => {
                ds.BasicInfoChanged += () => {
                    this.RaisePropertyChanged("CurrentActiveDs");
                };
            };
        }
    }
}
