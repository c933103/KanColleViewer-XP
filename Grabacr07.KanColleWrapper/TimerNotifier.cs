using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Internal;
using Livet;

namespace Grabacr07.KanColleWrapper
{
	/// <summary>
	/// 1 秒刻みのタイマー機能をサポートする変更通知オブジェクトを表します。
	/// </summary>
	public class TimerNotifier : NotificationObjectEx, IDisposable
	{
		#region static members

		private static readonly IConnectableObservable<long> timer;

		static TimerNotifier()
		{
			timer = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1)).Publish();
            timer.Connect();
		}

		#endregion

		private IDisposable subscriber;

        public TimerNotifier() : this(true) { }
        public TimerNotifier(bool connect) { if(connect) Connect(); }

        protected virtual void Tick() { }
        protected void Connect()
        {
            if (subscriber != null) return;

            var subscription = timer.Subscribe(_ => Tick());
            if(System.Threading.Interlocked.CompareExchange(ref subscriber, subscription, null) != null) {
                subscription.Dispose();
            }
        }

        protected void Disconnect()
        {
            System.Threading.Interlocked.Exchange(ref subscriber, null)?.Dispose();
        }
		
		public virtual void Dispose()
		{
            Disconnect();
        }
	}
}
