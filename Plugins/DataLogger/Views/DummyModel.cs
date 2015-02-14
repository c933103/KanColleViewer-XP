using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.ViewModels;

namespace LynLogger.Viewer
{
    public class DummyModel : TabItemViewModel
    {
        public override string Name { get { return "占位"; } protected set { throw new NotImplementedException(); } }
    }
}
