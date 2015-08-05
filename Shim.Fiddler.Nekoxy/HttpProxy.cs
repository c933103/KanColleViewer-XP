using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fiddler;
using System.Net;

namespace Nekoxy
{
    public static class HttpProxy
    {
        public static event Action<Session> AfterSessionComplete;
        
        public static bool IsEnableUpstreamProxy { get; set; }
        public static string UpstreamProxyHost { get; set; }
        public static int UpstreamProxyPort { get; set; }

        public static bool IsInListening { get; private set; }

        public static void Startup(int listeningPort, bool useIpV6 = false, bool isSetIEProxySettings = true)
        {
            FiddlerApplication.AfterSessionComplete += TranslateSession;
            FiddlerApplication.BeforeRequest += ConfigProxy;

            try {
                if (isSetIEProxySettings) {
                    WinInetUtil.SetProxyInProcessByUrlmon(listeningPort);
                    var systemProxyHost = WinInetUtil.GetSystemHttpProxyHost();
                    var systemProxyPort = WinInetUtil.GetSystemHttpProxyPort();
                    if (systemProxyPort != listeningPort || systemProxyHost.IsLoopbackHost()) {
                        _systemProxyHost = systemProxyHost;
                        _systemProxyPort = systemProxyPort;
                    }
                }

                FiddlerApplication.Startup(listeningPort, FiddlerCoreStartupFlags.OptimizeThreadPool);
            } catch (Exception) {
                Shutdown();
                throw;
            }
        }

        public static void Shutdown()
        {
            FiddlerApplication.Shutdown();

            FiddlerApplication.AfterSessionComplete -= TranslateSession;
            FiddlerApplication.BeforeRequest -= ConfigProxy;
        }

        private static void TranslateSession(Fiddler.Session sess)
        {
            var handler = AfterSessionComplete;
            if (handler == null) return;

            if (sess.RequestMethod == "CONNECT") return;
            if (string.IsNullOrWhiteSpace(sess.RequestMethod)) return;

            sess.utilDecodeRequest();
            sess.utilDecodeResponse();

            handler(new Session(sess));
        }

        private static void ConfigProxy(Fiddler.Session sess)
        {
            if(IsEnableUpstreamProxy && UpstreamProxyHost != null) {
                sess["X-OverrideGateway"] = new DnsEndPoint(UpstreamProxyHost, UpstreamProxyPort).ToString();
            } else if(!IsEnableUpstreamProxy && _systemProxyHost != null) {
                sess["X-OverrideGateway"] = new DnsEndPoint(_systemProxyHost, _systemProxyPort).ToString();
            }
        }

        private static string _systemProxyHost;
        private static int _systemProxyPort;
    }
}
