using Livet.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
    public class BrowserViewModel : TabItemViewModel
    {
        public override string Name
        {
            get { return "浏览器"; }
            protected set { throw new NotImplementedException(); }
        }

        public NavigatorViewModel Navigator => App.ViewModelRoot.Navigator;

        public new InteractionMessenger Messenger => App.ViewModelRoot.Messenger;
    }
}
