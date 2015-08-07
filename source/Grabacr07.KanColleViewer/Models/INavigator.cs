using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Models
{
	public interface INavigator
	{
		Uri Source { get; set; }
		bool IsNavigating { get; set; }
		bool CanGoBack { get; set; }
		bool CanGoForward { get; set; }

		event EventHandler GoBackRequested;
		event EventHandler GoForwardRequested;
		event EventHandler RefreshRequested;
		event EventHandler<UriEventArgs> UriRequested;
	}

    public class UriEventArgs : EventArgs
    {
        private Uri _uri;

        public static implicit operator Uri(UriEventArgs e)
        {
            return e._uri;
        }

        public static implicit operator UriEventArgs(Uri e)
        {
            return new UriEventArgs { _uri = e };
        }
    }
}
