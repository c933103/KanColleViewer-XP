﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleViewer.ViewModels.Composition;
using Grabacr07.KanColleViewer.ViewModels.Messages;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;
using Livet.Messaging;
using Livet.Messaging.IO;
using MetroRadiance;
using Settings = Grabacr07.KanColleViewer.Models.Settings;
using System.Windows.Input;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class SettingsViewModel : TabItemViewModel
	{
		public override string Name
		{
			get { return Resources.Settings; }
			protected set { throw new NotImplementedException(); }
		}

		#region ScreenshotFolder 変更通知プロパティ

		public string ScreenshotFolder
		{
			get { return Settings.Current.ScreenshotFolder; }
			set
			{
				if (Settings.Current.ScreenshotFolder != value)
				{
					Settings.Current.ScreenshotFolder = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged(nameof(this.CanOpenScreenshotFolder));
				}
			}
		}

		#endregion

		#region CanOpenScreenshotFolder 変更通知プロパティ

		public bool CanOpenScreenshotFolder => Directory.Exists(this.ScreenshotFolder);

		#endregion

		#region ScreenshotImageFormat 変更通知プロパティ

		public SupportedImageFormat ScreenshotImageFormat
		{
			get { return Settings.Current.ScreenshotImageFormat; }
			set
			{
				if (Settings.Current.ScreenshotImageFormat != value)
				{
					Settings.Current.ScreenshotImageFormat = value;
					this.RaisePropertyChanged();
				}
			}
		}

        #endregion

        public IEnumerable<BindableTextViewModel> Libraries { get; }
        public IEnumerable<CultureViewModel> Cultures { get; }

		#region Culture 変更通知プロパティ

		/// <summary>
		/// カルチャを取得または設定します。
		/// </summary>
		public string Culture
		{
			get { return Settings.Current.Culture; }
			set
			{
				if (Settings.Current.Culture != value)
				{
					ResourceService.Current.ChangeCulture(value);
					this.RaisePropertyChanged();
				}
			}
		}

        #endregion

        #region Flash相关

        public FlashQuality[] FlashQualitySettings { get { return (FlashQuality[])Enum.GetValues(typeof(FlashQuality)); } }
        public FlashRenderMode[] FlashRenderModes { get { return (FlashRenderMode[])Enum.GetValues(typeof(FlashRenderMode)); } }

        private ICommand _setFlashQuality = null;
        public ICommand SetFlashQuality { get { return _setFlashQuality ?? (_setFlashQuality = new SetFlashQualityCommand()); } }
        public FlashQuality FlashQuality { get { return Settings.Current.FlashQuality; } set { Settings.Current.FlashQuality = value; } }

        private ICommand _setFlashRenderMode = null;
        public ICommand SetFlashRenderMode { get { return _setFlashRenderMode ?? (_setFlashRenderMode = new SetFlashRenderModeCommand()); } }
        public FlashRenderMode FlashRenderMode { get { return Settings.Current.FlashRenderMode; } set { Settings.Current.FlashRenderMode = value; } }

        #endregion

        #region BrowserZoomFactor 変更通知プロパティ

        private BrowserZoomFactor _BrowserZoomFactor;

		public BrowserZoomFactor BrowserZoomFactor
		{
			get { return this._BrowserZoomFactor; }
			private set
			{
				if (this._BrowserZoomFactor != value)
				{
					this._BrowserZoomFactor = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region AllPlugins 変更通知プロパティ

		private List<PluginViewModel> _AllPlugins;

		public List<PluginViewModel> AllPlugins
		{
			get { return this._AllPlugins; }
			set
			{
				if (this._AllPlugins != value)
				{
					this._AllPlugins = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region BlacklistedPlugins 変更通知プロパティ

		private ReadOnlyDispatcherCollection<BlacklistedPluginViewModel> _BlacklistedPlugins;

		public ReadOnlyDispatcherCollection<BlacklistedPluginViewModel> BlacklistedPlugins
		{
			get { return this._BlacklistedPlugins; }
			set
			{
				if (this._BlacklistedPlugins != value)
				{
					this._BlacklistedPlugins = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		#region ViewRangeSettingsCollection 変更通知プロパティ

		private List<ViewRangeSettingsViewModel> _ViewRangeSettingsCollection;

		public List<ViewRangeSettingsViewModel> ViewRangeSettingsCollection
		{
			get { return this._ViewRangeSettingsCollection; }
			set
			{
				if (this._ViewRangeSettingsCollection != value)
				{
					this._ViewRangeSettingsCollection = value;
					this.RaisePropertyChanged();
				}
			}
		}

        #endregion

        private long _receivedBytes = 0;
        public long ReceivedBytes { get { return _receivedBytes; } }

        private long _sentBytes = 0;
        public long SentBytes { get { return _sentBytes; } }

		public SettingsViewModel()
		{
			if (Helper.IsInDesignMode) return;

			this.Libraries = ProductInfo.Libraries.Aggregate(
				new List<BindableTextViewModel>(),
				(list, lib) =>
				{
					list.Add(new BindableTextViewModel { Text = list.Count == 0 ? "Build with " : ", " });
					list.Add(new HyperlinkViewModel { Text = lib.Name.Replace(' ', Convert.ToChar(160)), Uri = lib.Url });
					// プロダクト名の途中で改行されないように、space を non-break space に置き換えてあげてるんだからねっっ
					return list;
				});

			this.Cultures = new[] { new CultureViewModel { DisplayName = "(auto)" } }
				.Concat(ResourceService.Current.SupportedCultures
					.Select(x => new CultureViewModel { DisplayName = x.EnglishName, Name = x.Name })
					.OrderBy(x => x.DisplayName))
				.ToList();

			this.CompositeDisposable.Add(new PropertyChangedEventListener(Settings.Current)
			{
				(sender, args) => this.RaisePropertyChanged(args.PropertyName),
			});

			var zoomFactor = new BrowserZoomFactor { Current = Settings.Current.BrowserZoomFactor };
			this.CompositeDisposable.Add(new PropertyChangedEventListener(zoomFactor)
			{
				{ nameof(zoomFactor.Current), (sender, args) => Settings.Current.BrowserZoomFactor = zoomFactor.Current },
			});
			this.BrowserZoomFactor = zoomFactor;

			this.ViewRangeSettingsCollection = ViewRangeCalcLogic.Logics
				.Select(x => new ViewRangeSettingsViewModel(x))
				.ToList();

			this.ReloadPlugins();

            Fiddler.FiddlerApplication.OnReadRequestBuffer += (_, e) => { System.Threading.Interlocked.Add(ref _sentBytes, e.iCountOfBytes);RaisePropertyChanged(nameof(SentBytes)); };
            Fiddler.FiddlerApplication.OnReadResponseBuffer += (_, e) => { System.Threading.Interlocked.Add(ref _receivedBytes, e.iCountOfBytes); RaisePropertyChanged(nameof(ReceivedBytes)); };
            Settings.Current.PropertyChanged += (o, e) => {
                if (e.PropertyName == nameof(Settings.Current.FlashQuality)) RaisePropertyChanged(e.PropertyName);
                if (e.PropertyName == nameof(Settings.Current.FlashRenderMode)) RaisePropertyChanged(e.PropertyName);
            };
		}


		public void OpenScreenshotFolderSelectionDialog()
		{
			var message = new FolderSelectionMessage("OpenFolderDialog/Screenshot")
			{
				Title = Resources.Settings_Screenshot_FolderSelectionDialog_Title,
				DialogPreference = Helper.IsWindows8OrGreater
					? FolderSelectionDialogPreference.CommonItemDialog
					: FolderSelectionDialogPreference.FolderBrowser,
				SelectedPath = this.CanOpenScreenshotFolder
					? this.ScreenshotFolder
					: ""
			};
			this.Messenger.Raise(message);

			if (Directory.Exists(message.Response))
			{
				this.ScreenshotFolder = message.Response;
			}
		}

		public void OpenScreenshotFolder()
		{
			if (this.CanOpenScreenshotFolder)
			{
				try
				{
					Process.Start(this.ScreenshotFolder);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);
				}
			}
		}

		public void ClearZoomFactor()
		{
			Application.Instance.MainWindowViewModel.Messenger.Raise(new InteractionMessage { MessageKey = "WebBrowser.Zoom" });
		}

		public void SetLocationLeft()
		{
			Application.Instance.MainWindowViewModel.Messenger.Raise(new SetWindowLocationMessage { MessageKey = "Window.Location", Left = 0.0 });
		}


		public void ReloadPlugins()
		{
			this.AllPlugins = new List<PluginViewModel>(
				PluginHost.Instance.Plugins.Select(x => new PluginViewModel(x)));

			var collection = ViewModelHelper.CreateReadOnlyDispatcherCollection(
				Settings.Current.BlacklistedPlugins,
				x => new BlacklistedPluginViewModel(x),
				System.Windows.Application.Current.Dispatcher);
			this.CompositeDisposable.Add(collection);
			this.BlacklistedPlugins = collection;
		}


		public class ViewRangeSettingsViewModel
		{
			private bool selected;

			public ICalcViewRange Logic { get; set; }

			public bool Selected
			{
				get { return this.selected; }
				set
				{
					this.selected = value;
					if (value)
					{
						Settings.Current.KanColleClientSettings.ViewRangeCalcType = this.Logic.Id;
					}
				}
			}

			public ViewRangeSettingsViewModel(ICalcViewRange logic)
			{
				this.Logic = logic;
				this.selected = Settings.Current.KanColleClientSettings.ViewRangeCalcType == logic.Id;
			}
        }
        
        private class SetFlashQualityCommand : ICommand
        {
            public event EventHandler CanExecuteChanged { add { } remove { } }
            public bool CanExecute(object parameter) { return true; }

            public void Execute(object parameter)
            {
                if (parameter is FlashQuality) {
                    Settings.Current.FlashQuality = (FlashQuality)parameter;
                }
            }
        }

        private class SetFlashRenderModeCommand : ICommand
        {
            public event EventHandler CanExecuteChanged { add { } remove { } }
            public bool CanExecute(object parameter) { return true; }

            public void Execute(object parameter)
            {
                if (parameter is FlashRenderMode) {
                    Settings.Current.FlashRenderMode = (FlashRenderMode)parameter;
                }
            }
        }
    }
}
