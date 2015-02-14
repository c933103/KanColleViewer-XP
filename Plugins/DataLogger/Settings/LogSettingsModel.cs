using Grabacr07.KanColleViewer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Settings
{
    class LogSettingsModel : TabItemViewModel
    {

        public override string Name
        {
            get { return "记录间隔"; }
            protected set { throw new NotImplementedException(); }
        }
    }
}
