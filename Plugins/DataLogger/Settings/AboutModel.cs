using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.ViewModels;
using LynLogger.Properties;

namespace LynLogger.Settings
{
    public class AboutModel : TabItemViewModel
    {
        public override string Name { get { return Resources.AboutTabName; } protected set { throw new NotImplementedException(); } }
    }
}
