using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Views.Controls
{
    public class TabViewItem : Models.NotificationSourceObject
    {
        public virtual string TabName { get; set; }

        private object _view;
        public virtual object TabView => _view ?? (_view = _viewFactory?.Invoke());

        private Func<object> _viewFactory;
        public Func<object> ViewFactory
        {
            get { return _viewFactory; }
            set
            {
                _view = null;
                _viewFactory = value;
                RaisePropertyChanged(nameof(TabView));
            }
        }

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

        public TabViewItem(string name, Func<object> viewFactory)
        {
            TabName = name;
            ViewFactory = viewFactory; ;
        }
    }
}
