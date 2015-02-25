using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace LynLogger
{
    [Export(typeof(IToolPlugin))]
    [ExportMetadata("Title", "LynLogger")]
    [ExportMetadata("Description", "Test")]
    [ExportMetadata("Version", "1.0")]
    [ExportMetadata("Author", "@Linnaea")]
    public class LynLoggerMain : IToolPlugin, IDisposable
    {
        private HashSet<IDisposable> _disposables = new HashSet<IDisposable>();
        private ToolsModel model = new ToolsModel();

        public LynLoggerMain()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            _disposables.Add(KanColleClient.Current.Proxy.api_port.TryParse<kcsapi_port>().Subscribe(new Observers.ApiPortObserver()));
        }

        System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if(args.Name == typeof(LynLoggerMain).Assembly.FullName) {
                return typeof(LynLoggerMain).Assembly;
            }
            return null;
        }

        public string ToolName
        {
            get { return "LynLogger"; }
        }

        public object GetToolView()
        {
            return new ToolsView() { DataContext = model };
        }

        public object GetSettingsView()
        {
            return null;
        }

        ~LynLoggerMain()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if(!disposing) return;
            foreach(var d in _disposables) {
                d.Dispose();
            }
            Models.DataStore.SaveData();
            GC.SuppressFinalize(this);
        }
    }
}
