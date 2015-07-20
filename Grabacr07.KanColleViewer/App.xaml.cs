using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.ViewModels;
using Grabacr07.KanColleViewer.Views;
using Grabacr07.KanColleWrapper;
using Livet;
using MetroRadiance;
using AppSettings = Grabacr07.KanColleViewer.Properties.Settings;
using Settings = Grabacr07.KanColleViewer.Models.Settings;
using System.Reflection;

namespace Grabacr07.KanColleViewer
{
	public partial class App
	{
		public static ProductInfo ProductInfo { get; private set; }
		public static MainWindowViewModel ViewModelRoot { get; private set; }

		static App()
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, args) => ReportException(sender, args.ExceptionObject as Exception, true);
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
		}

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName resolvName = new AssemblyName(args.Name);
            return AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetName().Name == resolvName.Name).FirstOrDefault();
        }

        protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			this.DispatcherUnhandledException += (sender, args) => ReportException(sender, args.Exception, false);

            System.Windows.Media.Animation.Timeline.DesiredFrameRateProperty.OverrideMetadata(
                typeof(System.Windows.Media.Animation.Timeline),
                new FrameworkPropertyMetadata { DefaultValue = 20 }
            );

            DispatcherHelper.UIDispatcher = this.Dispatcher;
			ProductInfo = new ProductInfo();

			Settings.Load();
			PluginHost.Instance.Initialize();
			NotifierHost.Instance.Initialize(KanColleClient.Current);
            Helper.SetRegistryFeatureBrowserEmulation();
            Helper.SetMMCSSTask();

			KanColleClient.Current.Proxy.Startup();
			KanColleClient.Current.Proxy.UpstreamProxySettings = Settings.Current.ProxySettings;

            ResourceService.Current.ChangeCulture(Settings.Current.Culture);
			ThemeService.Current.Initialize(this, Theme.Dark, Accent.Purple);

            ViewModelRoot = new MainWindowViewModel();
            this.MainWindow = new MainWindow { DataContext = ViewModelRoot };
            ViewModelRoot.UpdateLayout(Settings.Current.LRSplit);
			this.MainWindow.Show();
		}

        protected override void OnExit(ExitEventArgs e)
		{
            Settings.Current.Save();
            base.OnExit(e);

            KanColleClient.Current.Proxy.Shutdown();
            PluginHost.Instance.Dispose();
        }

		private static void ReportException(object sender, Exception exception, bool fatal)
		{
			#region const
			const string messageFormat = @"
===========================================================
ERROR, date = {0}, sender = {1},
{2}
";
			const string path = "error.log";
            #endregion

            try {
                while(exception != null) {
                    var message = string.Format(messageFormat, DateTimeOffset.Now, sender, exception);

                    if(fatal) MessageBox.Show(message, "Unhandled exception", MessageBoxButton.OK, MessageBoxImage.Error);
                    Debug.WriteLine(message);
                    File.AppendAllText(path, message);
                    exception = exception.InnerException;
                }
            } catch (Exception ex) {
                if(fatal) MessageBox.Show(ex.ToString(), "Unhandled exception", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine(ex);
            }
        }
    }
}
