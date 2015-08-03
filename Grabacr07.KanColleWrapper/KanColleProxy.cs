using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Nekoxy;
using Fiddler;
using Livet;

namespace Grabacr07.KanColleWrapper
{
	public partial class KanColleProxy
	{
		private readonly IConnectableObservable<Nekoxy.Session> connectableSessionSource;
		private readonly IConnectableObservable<Nekoxy.Session> apiSource;
		private readonly LivetCompositeDisposable compositeDisposable;
        private readonly Dictionary<string, Action<Fiddler.Session>> localRequestHandlers = new Dictionary<string, Action<Fiddler.Session>>();

        public bool Synchronize { get; set; }

		public IObservable<Nekoxy.Session> SessionSource => connectableSessionSource;
		public IObservable<Nekoxy.Session> ApiSessionSource => apiSource;

		public IProxySettings UpstreamProxySettings { get; set; }

		public KanColleProxy()
		{
			this.compositeDisposable = new LivetCompositeDisposable();

			this.connectableSessionSource = Observable
				.FromEvent<Action<Nekoxy.Session>, Nekoxy.Session>(
					action => action,
					h => HttpProxy.AfterSessionComplete += h,
					h => HttpProxy.AfterSessionComplete -= h)
				.Publish();

			this.apiSource = this.connectableSessionSource
                .Where(s => s.Request.PathAndQuery.StartsWith("/kcsapi") && s.Response.MimeType.Equals("text/plain"))
                .SynchronizeFIFO((a, b, c) => Synchronize)
            #region .Do(debug)
#if DEBUG
                .Do(session =>
				{
					Debug.WriteLine("==================================================");
					Debug.WriteLine("Fiddler session: ");
					Debug.WriteLine(session);
					Debug.WriteLine("");
				})
#endif
			#endregion
				.Publish();
		}


		public void Startup(int proxy = 0)
		{
            if(proxy < 1024) proxy = new Random().Next(1024, 65535);

            FiddlerApplication.BeforeRequest += this.Fiddler_BeforeRequest;
            HttpProxy.Startup(proxy);
            this.compositeDisposable.Add(this.connectableSessionSource.Connect());
			this.compositeDisposable.Add(this.apiSource.Connect());
		}

        public void Shutdown()
		{
			this.compositeDisposable.Dispose();
            HttpProxy.Shutdown();
            FiddlerApplication.BeforeRequest -= this.Fiddler_BeforeRequest;
        }

		/// <summary>
		/// Fiddler からのリクエスト発行時にプロキシを挟む設定を行います。
		/// </summary>
		/// <param name="requestingSession">通信を行おうとしているセッション。</param>
		private void Fiddler_BeforeRequest(Fiddler.Session requestingSession)
		{
            if(requestingSession.hostname == "kancolleviewer.local") {
                requestingSession.utilCreateResponseAndBypassServer();
                var path = requestingSession.PathAndQuery;
                var queryIndex = path.IndexOf('?');
                if(queryIndex >= 0) {
                    path = path.Substring(0, queryIndex);
                }

                Action<Fiddler.Session> handler;
                if (localRequestHandlers.TryGetValue(path, out handler)) {
                    requestingSession.oResponse.headers.HTTPResponseCode = 200;
                    requestingSession.oResponse.headers.HTTPResponseStatus = "200 OK";
                    handler?.Invoke(requestingSession);
                } else {
                    requestingSession.oResponse.headers.HTTPResponseCode = 410;
                    requestingSession.oResponse.headers.HTTPResponseStatus = "410 Gone";
                }
                return;
            }

			var settings = this.UpstreamProxySettings;
            if (settings == null) return;

            var compiled = settings.CompiledRules;
            if (compiled == null) settings.CompiledRules = compiled = ProxyRule.CompileRule(settings.Rules);
            var result = ProxyRule.ExecuteRules(compiled, requestingSession.RequestMethod, new Uri(requestingSession.fullUrl));

            if(result.Action == ProxyRule.MatchAction.Block) {
                requestingSession.utilCreateResponseAndBypassServer();
                requestingSession.oResponse.headers.HTTPResponseCode = 450;
                requestingSession.oResponse.headers.HTTPResponseStatus = "450 Blocked As Requested";
                return;
            }

            if(KanColleClient.Current.Settings.DisallowSortieWithHeavyDamage) {
                if(KanColleClient.Current.Homeport.Organization.Fleets.Any(x => x.Value.IsInSortie && x.Value.State.Situation.HasFlag(Models.FleetSituation.HeavilyDamaged))) {
                    if (    requestingSession.PathAndQuery.Length > 7
                         && requestingSession.PathAndQuery.Substring(0, 7) == "/kcsapi"
                         && (requestingSession.PathAndQuery.Contains("battle") || requestingSession.PathAndQuery.Contains("sortie"))
                         && !requestingSession.PathAndQuery.Contains("practice")
                         && !requestingSession.PathAndQuery.Contains("result")) {

                        requestingSession.utilCreateResponseAndBypassServer();
                        requestingSession.oResponse.headers.HTTPResponseCode = 450;
                        requestingSession.oResponse.headers.HTTPResponseStatus = "450 Blocked As Requested";
                        return;
                    }
                }
            }

            if(result.Action == ProxyRule.MatchAction.Proxy && result.Proxy != null) {
                requestingSession["X-OverrideGateway"] = result.Proxy;
                if(result.ProxyAuth != null && !requestingSession.RequestHeaders.Exists("Proxy-Authorization")) {
                    requestingSession["X-OverrideGateway"] = result.Proxy;
                    requestingSession.RequestHeaders.Add("Proxy-Authorization", result.ProxyAuth);
                }
            }

            requestingSession.bBufferResponse = false;
        }

        public void SetLocalRerquestHandler(string path, Action<Fiddler.Session> proc)
        {
            if (proc != null) {
                localRequestHandlers[path] = proc;
            } else {
                localRequestHandlers.Remove(path);
            }
        }
	}
}
