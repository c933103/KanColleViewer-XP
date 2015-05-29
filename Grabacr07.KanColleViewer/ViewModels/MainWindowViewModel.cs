using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.ViewModels.Messages;
using Grabacr07.KanColleWrapper;
using Livet;
using Livet.EventListeners;
using Livet.Messaging.Windows;
using MetroRadiance;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class MainWindowViewModel : WindowViewModel
	{
		private Mode currentMode;
		private MainContentViewModel mainContent;

		public NavigatorViewModel Navigator { get; private set; }
		public SettingsViewModel Settings { get; private set; }

		#region Mode 変更通知プロパティ

		public Mode Mode
		{
			get { return this.currentMode; }
			set
			{
				this.currentMode = value;
				switch (value)
				{
					case Mode.NotStarted:
						this.Content = StartContentViewModel.Instance;
						this.StatusBar = StartContentViewModel.Instance;
						StatusService.Current.Set(Properties.Resources.StatusBar_NotStarted);
						ThemeService.Current.ChangeAccent(Accent.Purple);
						break;
					case Mode.Started:
						this.Content = this.mainContent ?? (this.mainContent = new MainContentViewModel());
						StatusService.Current.Set(Properties.Resources.StatusBar_Ready);
						ThemeService.Current.ChangeAccent(Accent.Blue);
						break;
					case Mode.InSortie:
						ThemeService.Current.ChangeAccent(Accent.Orange);
						break;
				}

				this.RaisePropertyChanged();
			}
		}

		#endregion

		#region Content 変更通知プロパティ

		private ViewModel _Content;

		public ViewModel Content
		{
			get { return this._Content; }
			set
			{
				if (this._Content != value)
				{
					this._Content = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region StatusMessage 変更通知プロパティ

		private string _StatusMessage;

		public string StatusMessage
		{
			get { return this._StatusMessage; }
			set
			{
				if (this._StatusMessage != value)
				{
					this._StatusMessage = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region StatusBar 変更通知プロパティ

		private ViewModel _StatusBar;

		public ViewModel StatusBar
		{
			get { return this._StatusBar; }
			set
			{
				if (this._StatusBar != value)
				{
					this._StatusBar = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region TopMost 変更通知プロパティ

		public bool TopMost
		{
			get { return Models.Settings.Current.TopMost; }
			set
			{
				if (Models.Settings.Current.TopMost != value)
				{
					Models.Settings.Current.TopMost = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

        public bool LRSplit
        {
            get { return Models.Settings.Current.LRSplit; }
            set
            {
                if(Models.Settings.Current.LRSplit != value) {
                    Models.Settings.Current.LRSplit = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private bool _downloadActivity = false;
        public bool DownloadActive { get { return _downloadActivity = !_downloadActivity; } set { RaisePropertyChanged(); } }
        private bool _uploadActivity = false;
        public bool UploadActive { get { return _uploadActivity = !_uploadActivity; } set { RaisePropertyChanged(); } }

        private volatile int _outstandingRequests = 0;
        public int OutstandingRequests { get { return _outstandingRequests; } }

        public MainWindowViewModel()
		{
			this.Title = App.ProductInfo.Title;
			this.Navigator = new NavigatorViewModel();
			this.Settings = new SettingsViewModel();

			this.CompositeDisposable.Add(new PropertyChangedEventListener(StatusService.Current)
			{
				{ () => StatusService.Current.Message, (sender, args) => this.StatusMessage = StatusService.Current.Message },
			});
			this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current)
			{
				{ () => KanColleClient.Current.IsStarted, (sender, args) => this.UpdateMode() },
				{ () => KanColleClient.Current.IsInSortie, (sender, args) => this.UpdateMode() },
			});

			this.UpdateMode();
            Models.Settings.Current.PropertyChanged += (_, __) => this.UpdateLayout(Models.Settings.Current.LRSplit);

            Fiddler.FiddlerApplication.OnReadResponseBuffer += (_, e) => DownloadActive = true;
            Fiddler.FiddlerApplication.OnReadRequestBuffer += (_, e) => UploadActive = true;
            Fiddler.FiddlerApplication.BeforeRequest += _ => { System.Threading.Interlocked.Increment(ref _outstandingRequests); RaisePropertyChanged(nameof(OutstandingRequests)); };
            Fiddler.FiddlerApplication.BeforeResponse += _ => { System.Threading.Interlocked.Decrement(ref _outstandingRequests); RaisePropertyChanged(nameof(OutstandingRequests)); };
            Fiddler.FiddlerApplication.BeforeReturningError += _ => { if (_.responseCode == 408) return; System.Threading.Interlocked.Decrement(ref _outstandingRequests); RaisePropertyChanged(nameof(OutstandingRequests)); };
        }

		public void TakeScreenshot()
		{
			var path = Helper.CreateScreenshotFilePath();
			var message = new ScreenshotMessage("Screenshot/Save") { Path = path, };

			this.Messenger.Raise(message);

			var notify = message.Response.IsSuccess
				? Properties.Resources.Screenshot_Saved + Path.GetFileName(path)
				: Properties.Resources.Screenshot_Failed + message.Response.Exception.Message;
			StatusService.Current.Notify(notify);
		}


		/// <summary>
		/// メイン ウィンドウをアクティブ化することを試みます。
		/// </summary>
		public void Activate()
		{
			this.Messenger.Raise(new WindowActionMessage(WindowAction.Active, "Window/Activate"));
		}


		private void UpdateMode()
		{
			this.Mode = KanColleClient.Current.IsStarted
				? KanColleClient.Current.IsInSortie
					? Mode.InSortie
					: Mode.Started
				: Mode.NotStarted;
		}

        internal void UpdateLayout(bool LR)
        {
            if(LR) {
                ((Views.MainWindow)App.Instance.MainWindow).Row0Height.Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star);
                ((Views.MainWindow)App.Instance.MainWindow).Row1Height.Height = System.Windows.GridLength.Auto;
                ((Views.MainWindow)App.Instance.MainWindow).Col0Width.Width = System.Windows.GridLength.Auto;
                ((Views.MainWindow)App.Instance.MainWindow).Col1Width.Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star);
            } else {
                ((Views.MainWindow)App.Instance.MainWindow).Row0Height.Height = System.Windows.GridLength.Auto;
                ((Views.MainWindow)App.Instance.MainWindow).Row1Height.Height = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star);
                ((Views.MainWindow)App.Instance.MainWindow).Col0Width.Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star);
                ((Views.MainWindow)App.Instance.MainWindow).Col1Width.Width = System.Windows.GridLength.Auto;
            }
        }
	}
}
