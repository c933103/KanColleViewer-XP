using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace DataLogger
{
    [Export(typeof(IToolPlugin))]
    [ExportMetadata("Title", "LynLogger")]
    [ExportMetadata("Description", "Test")]
    [ExportMetadata("Version", "1.0")]
    [ExportMetadata("Author", "@Linnaea")]
    public class LynLoggerLoader : IToolPlugin
    {
        public LynLoggerLoader()
        {
            KanColleClient.Current.Proxy.api_start2.Subscribe(_ => Logger.DataHolder.Init());
            KanColleClient.Current.Proxy.api_port.TryParse<kcsapi_port>().Subscribe(new Logger.Observers.APIPortObserver());
            //KanColleClient.Current.Homeport.Materials.
        }

        public string ToolName
        {
            get { return "LynLogger"; }
        }

        public object GetToolView()
        {
            return new ToolsView();
        }

        public object GetSettingsView()
        {
            return null;
        }
    }
}
