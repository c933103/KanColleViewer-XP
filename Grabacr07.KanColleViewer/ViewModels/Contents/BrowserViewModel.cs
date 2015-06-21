using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.ViewModels.Messages;
using Livet.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
    public class BrowserViewModel : TabItemViewModel
    {
        public override string Name
        {
            get { return "浏览器"; }
            protected set { throw new NotImplementedException(); }
        }

        public NavigatorViewModel Navigator { get; private set; }
        public VolumeViewModel Volume { get; private set; }

        public BrowserViewModel()
        {
            this.Navigator = new NavigatorViewModel();
            this.Volume = new VolumeViewModel();
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

        private bool _showNavigator = true;
        public bool ShowNavigator
        {
            get { return _showNavigator; }
            set
            {
                if (value == _showNavigator) return;
                _showNavigator = value;
                RaisePropertyChanged();
            }
        }

        public void ToggleNavigator()
        {
            ShowNavigator = !_showNavigator;
        }
    }
}
