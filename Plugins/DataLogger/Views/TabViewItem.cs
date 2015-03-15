using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Views
{
    public class TabViewItem : Models.NotificationSourceObject
    {
        public virtual string TabName { get; set; }
        public virtual object TabView { get; set; }

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
