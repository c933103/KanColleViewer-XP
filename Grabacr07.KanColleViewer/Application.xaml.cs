using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.ViewModels;
using Grabacr07.KanColleViewer.Views;
using Grabacr07.KanColleWrapper;
using Livet;
using MetroRadiance;
using Settings = Grabacr07.KanColleViewer.Models.Settings;
using System.Reflection;

namespace Grabacr07.KanColleViewer
{
	/// <summary>
	/// アプリケーションの状態を示す識別子を定義します。
	/// </summary>
	public enum ApplicationState
	{
		/// <summary>
		/// アプリケーションは起動中です。
		/// </summary>
		Startup,

		/// <summary>
		/// アプリケーションは起動準備が完了し、実行中です。
		/// </summary>
		Running,

		/// <summary>
		/// アプリケーションは終了したか、または終了処理中です。
		/// </summary>
		Terminate,
	}

	sealed partial class Application : INotifyPropertyChanged
	{
		static Application()
		{
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => ReportException(sender, args.ExceptionObject as Exception, true);
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName resolvName = new AssemblyName(args.Name);
            return AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetName().Name == resolvName.Name).FirstOrDefault();
        }

    private readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();
		private event PropertyChangedEventHandler PropertyChangedInternal;

		/// <summary>
		/// 現在の <see cref="AppDomain"/> の <see cref="Application"/> オブジェクトを取得します。
		/// </summary>
		public static new Application Current => (Application)System.Windows.Application.Current;

		/// <summary>
		/// アプリケーションのメイン ウィンドウを制御する <see cref="MainWindowViewModel"/> オブジェクトを取得します。
		/// </summary>
		public MainWindowViewModel MainWindowViewModel { get; private set; }

		/// <summary>
		/// アプリケーションの現在の状態を示す識別子を取得します。
		/// </summary>
		public ApplicationState State { get; private set; }


        protected override void OnStartup(StartupEventArgs e)
        {
            this.ChangeState(ApplicationState.Startup);

            this.DispatcherUnhandledException += (sender, args) => {
                ReportException(sender, args.Exception, false);
                args.Handled = true;
            };

            System.Windows.Media.Animation.Timeline.DesiredFrameRateProperty.OverrideMetadata(
typeof(System.Windows.Media.Animation.Timeline),
new FrameworkPropertyMetadata { DefaultValue = 20 }
            );

            DispatcherHelper.UIDispatcher = this.Dispatcher;

            Settings.Load();
            ResourceService.Current.ChangeCulture(Settings.Current.Culture);
            ThemeService.Current.Initialize(this, Theme.Dark, Accent.Purple);

            PluginHost.Instance.Initialize();
            NotifierHost.Instance.Initialize();
            Helper.SetRegistryFeatureBrowserEmulation();
            Helper.SetRegistryFeatureLegacyInputModel();
            Helper.SetMMCSSTask();

            this.MainWindowViewModel = new MainWindowViewModel();

            // Application.OnExit で破棄 (or 処理) するものたち
            this.compositeDisposable.Add(this.MainWindowViewModel);
            this.compositeDisposable.Add(PluginHost.Instance);
            this.compositeDisposable.Add(NotifierHost.Instance);
            this.compositeDisposable.Add(Settings.Current.Save);

            // BootstrapProxy() で Views.Settings.ProxyBootstrapper.Show() が呼ばれるより前に
            // Application.MainWindow を設定しておく。これ大事
            this.MainWindow = new MainWindow { DataContext = this.MainWindowViewModel, };
            MainWindowViewModel.UpdateLayout(Settings.Current.LRSplit);

            this.compositeDisposable.Add(KanColleClient.Current.Proxy.Shutdown);

            this.MainWindow.Show();

            base.OnStartup(e);
            this.ChangeState(ApplicationState.Running);

            if (Definitions.UnixTimestamp - Settings.Current.LastUpdateCheck > 86400) {
                var wc = new System.Net.WebClient();
                wc.DownloadStringTaskAsync(ProductInfo.UpdateCheckUri).ContinueWith(task => {
                    wc.Dispose();
                    if (task.Status == TaskStatus.RanToCompletion) {
                        Settings.Current.LastUpdateVersion = task.Result.Trim();
                        Settings.Current.LastUpdateCheck = Definitions.UnixTimestamp;
                    }
                });
            }
        }

		protected override void OnExit(ExitEventArgs e)
		{
			this.ChangeState(ApplicationState.Terminate);
			base.OnExit(e);

			this.compositeDisposable.Dispose();
		}

		/// <summary>
		/// <see cref="State"/> プロパティを更新し、<see cref="INotifyPropertyChanged.PropertyChanged"/> イベントを発生させます。
		/// </summary>
		/// <param name="value"></param>
		private void ChangeState(ApplicationState value)
		{
			if (this.State == value) return;

			this.State = value;
			this.RaisePropertyChanged(nameof(this.State));
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

            // ToDo: 例外ダイアログ

            try {
                while (exception != null) {
                    var message = string.Format(messageFormat, DateTimeOffset.Now, sender, exception);

                    if (fatal) MessageBox.Show(message, "Unhandled exception", MessageBoxButton.OK, MessageBoxImage.Error);
                    Debug.WriteLine(message);
                    File.AppendAllText(path, message);
                    exception = exception.InnerException;
                }
            } catch (Exception ex) {
                if (fatal) MessageBox.Show(ex.ToString(), "Unhandled exception", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine(ex);
            }
        }


		#region INotifyPropertyChanged members

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { this.PropertyChangedInternal += value; }
			remove { this.PropertyChangedInternal -= value; }
		}

		private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.PropertyChangedInternal?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}
