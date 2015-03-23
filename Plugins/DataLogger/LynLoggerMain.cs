using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models.Raw;
using LynLogger.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;

namespace LynLogger
{
    [Export(typeof(IToolPlugin))]
    [ExportMetadata("Title", "LynLogger")]
    [ExportMetadata("Description", "Test")]
    [ExportMetadata("Version", "1.0")]
    [ExportMetadata("Author", "@Linnaea")]
    public class LynLoggerMain : IToolPlugin, IDisposable
    {
        public static LynLoggerMain Instance { get; private set; }

        private static Action<LynLoggerMain> _onInstanceCreate;
        public static event Action<LynLoggerMain> OnInstanceCreate
        {
            add { _onInstanceCreate += value.MakeWeak(x => _onInstanceCreate -= x); }
            remove { }
        }

        private static readonly Logger.BasicInfoLogger bil = new Logger.BasicInfoLogger();
        private static readonly Logger.ShipDataLogger sdl = new Logger.ShipDataLogger();

        internal Observers.ApiBattleObserver BattleObserver { get; private set; }
        internal Observers.ApiBattleResultObserver BattleResultObserver { get; private set; }
        internal Observers.ApiMapStartNextObserver MapStartNextObserver { get; private set; }

        private LinkedList<IDisposable> _disposables = new LinkedList<IDisposable>();
        private ToolsModel model = new ToolsModel();

        public LynLoggerMain()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve; //Somehow BinaryFormatter can't find our assembly despite the fact that we're the one who called it.

            _disposables.AddLast(KanColleClient.Current.Proxy.api_port.TryParse<kcsapi_port>().Subscribe(new Observers.ApiPortObserver()));
            _disposables.AddLast(KanColleClient.Current.Proxy.api_get_member_ship2.TryParse<kcsapi_ship2[]>().Subscribe(new Observers.ApiShip2Observer()));

            BattleObserver = new Observers.ApiBattleObserver();
            _disposables.AddLast(KanColleClient.Current.Proxy.api_req_sortie_battle.Subscribe(BattleObserver));
            _disposables.AddLast(KanColleClient.Current.Proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_req_battle_midnight/battle").Subscribe(BattleObserver));
            _disposables.AddLast(KanColleClient.Current.Proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_req_practice/battle").Subscribe(BattleObserver));
            _disposables.AddLast(KanColleClient.Current.Proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_req_practice/midnight_battle").Subscribe(BattleObserver));
            _disposables.AddLast(KanColleClient.Current.Proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_req_battle_midnight/sp_midnight").Subscribe(BattleObserver));

            MapStartNextObserver = new Observers.ApiMapStartNextObserver();
            _disposables.AddLast(KanColleClient.Current.Proxy.api_req_map_start.Subscribe(MapStartNextObserver));
            _disposables.AddLast(KanColleClient.Current.Proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_req_map/next").Subscribe(MapStartNextObserver));

            BattleResultObserver = new Observers.ApiBattleResultObserver();
            //_disposables.AddLast(KanColleClient.Current.Proxy.api_req_sortie_battleresult.TryParse<kcsapi_battleresult>().Subscribe(BattleResultObserver));

            Instance = this;
            if(_onInstanceCreate != null) _onInstanceCreate(this);
        }

        System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if(new AssemblyName(args.Name).Name == typeof(LynLoggerMain).Assembly.GetName().Name) {
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
            Models.DataStore.SaveData();
            if(!disposing) return;
            foreach(var d in _disposables) {
                d.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }

    public abstract class CustomComparerBase<T> : MarshalByRefObject, IComparer<T>
    {
        public abstract int Compare(T x, T y);
    }

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public sealed class ComparerClassAttribute : Attribute
    {
        readonly string _className;
        
        public ComparerClassAttribute(string className)
        {
            _className = className;
        }

        public string ClassName
        {
            get { return _className; }
        }
    }
}
