﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.ViewModels;
using Livet;

namespace DataLogger
{
    public class ToolsModel : ViewModel
    {
        public IReadOnlyList<TabItemViewModel> Pages { get; private set; }

        private TabItemViewModel _page;
        public TabItemViewModel SelectedPage
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

        public ToolsModel()
        {
            var listPages = new List<TabItemViewModel>() {
                new Viewer.DummyModel(),
                new Settings.TelemetryModel(),
                new Settings.AboutModel()
            };
            listPages.ForEach(x => x.IsSelected = false);
            Pages = listPages;
            SelectedPage = Pages.First();
        }
    }
}