using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Grabacr07.KanColleWrapper
{
    public class MutexFIFO
    {
        private readonly object _syncRoot = new object();
        private readonly int _spinIteration;
        private readonly int _maxWaitTime = 15000;

        private volatile int _grantedTicket = 1;
        private volatile int _currentTicket = 0;

        private volatile int _acquiredThreadId = 0;
        private int _reentrance = 0;

        private ThreadLocal<int> _ourTicket = new ThreadLocal<int>(() => 0, false);

        public MutexFIFO() : this(10000) { }
        public MutexFIFO(int spinIter) { _spinIteration = spinIter; }

        public void Enter()
        {
            if(_ourTicket.Value == _grantedTicket) {
                _reentrance++;
                System.Diagnostics.Debug.WriteLine(string.Format("[{0:X16}] [MutexFIFO {1:X8}] [Thread {2}] Ticket {3}: {4} reentrance",
                                                                DateTime.UtcNow.ToFileTime(), GetHashCode(), Thread.CurrentThread.ManagedThreadId, _grantedTicket, _reentrance));
                return;
            }

            var iter = 0;
            var ticket = _ourTicket.Value = Interlocked.Increment(ref _currentTicket);
            for (iter = 0; iter < _spinIteration; iter++) {
                if (_grantedTicket == ticket) goto _acquired;
            }

            lock (_syncRoot) {
                System.Diagnostics.Debug.WriteLine(string.Format("[{0:X16}] [MutexFIFO {1:X8}] [Thread {2}] Unable to acquire mutex after {3} spins, currently held by thread {4}, current ticket is {5}, waiting for ticket {6}",
                                                                DateTime.UtcNow.ToFileTime(), GetHashCode(), Thread.CurrentThread.ManagedThreadId, iter, _acquiredThreadId, _grantedTicket, ticket));
                while (true) {
                    int waitTime = 100;
                    if (_grantedTicket == ticket) goto _acquired;
                    if(!Monitor.Wait(_syncRoot, waitTime)) {
                        System.Diagnostics.Debug.WriteLine(string.Format("[{0:X16}] [MutexFIFO {1:X8}] [Thread {2}] Unable to acquire mutex after {3} ms, currently held by thread {4}, current ticket is {5}, waiting for ticket {6}",
                                                                        DateTime.UtcNow.ToFileTime(), GetHashCode(), Thread.CurrentThread.ManagedThreadId, waitTime, _acquiredThreadId, _grantedTicket, ticket));
                        waitTime *= 2;
                        if (waitTime > _maxWaitTime) waitTime = _maxWaitTime;
                    }
                }
            }

            _acquired:
            System.Diagnostics.Debug.WriteLine(string.Format("[{0:X16}] [MutexFIFO {1:X8}] [Thread {2}] Acquired ticket {3} after {4} spins",
                                                            DateTime.UtcNow.ToFileTime(), GetHashCode(), Thread.CurrentThread.ManagedThreadId, ticket, iter));
            _acquiredThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public void Exit()
        {
            if (_ourTicket.Value != _grantedTicket)
                throw new SynchronizationLockException();

            if (_reentrance > 0) {
                _reentrance--;
                System.Diagnostics.Debug.WriteLine(string.Format("[{0:X16}] [MutexFIFO {1:X8}] [Thread {2}] Ticket {3}: {4} reentrance",
                                                                DateTime.UtcNow.ToFileTime(), GetHashCode(), Thread.CurrentThread.ManagedThreadId, _grantedTicket, _reentrance));
                return;
            }

            _acquiredThreadId = 0;
            _ourTicket.Value = 0;

            System.Diagnostics.Debug.WriteLine(string.Format("[{0:X16}] [MutexFIFO {1:X8}] [Thread {2}] Releasing ticket {3}",
                                                            DateTime.UtcNow.ToFileTime(), GetHashCode(), Thread.CurrentThread.ManagedThreadId, _grantedTicket));
            Interlocked.Increment(ref _grantedTicket);

            lock (_syncRoot) {
                Monitor.PulseAll(_syncRoot);
            }
        }
    }

    static class RxExtension
    {
        public static IObservable<T> SynchronizeFIFO<T>(this IObservable<T> source, Func<T, bool, Exception, bool> syncPredicate)
        {
            return new SyncFIFOObservable<T>(source, syncPredicate);
        }

        private class SyncFIFOObservable<T> : IObservable<T>
        {
            private readonly IObservable<T> _source;
            private readonly MutexFIFO _syncLock;
            private readonly Func<T, bool, Exception, bool> _syncPredicate;

            public SyncFIFOObservable(IObservable<T> src)
                : this(src, new MutexFIFO()) { }

            public SyncFIFOObservable(IObservable<T> src, int spinCount)
                : this(src, new MutexFIFO(spinCount)) { }

            public SyncFIFOObservable(IObservable<T> src, MutexFIFO syncLock)
                : this(src, syncLock, (a,b,c) => true) { }

            public SyncFIFOObservable(IObservable<T> src, Func<T, bool, Exception, bool> syncPredicate)
                : this(src, new MutexFIFO(), syncPredicate) { }

            public SyncFIFOObservable(IObservable<T> src, int spinCount, Func<T, bool, Exception, bool> syncPredicate)
                : this(src, new MutexFIFO(spinCount), syncPredicate) { }

            public SyncFIFOObservable(IObservable<T> src, MutexFIFO syncLock, Func<T, bool, Exception, bool> syncPredicate)
            { _source = src; _syncLock = syncLock; _syncPredicate = syncPredicate; }

            public IDisposable Subscribe(IObserver<T> observer)
            {
                return _source.Subscribe(new Sink(this, observer));
            }

            private class Sink : IObserver<T>
            {
                private readonly SyncFIFOObservable<T> _parent;
                private readonly IObserver<T> _subscriber;

                public Sink(SyncFIFOObservable<T> parent, IObserver<T> subscriber) { _parent = parent; _subscriber = subscriber; }

                public void OnCompleted()
                {
                    var locked = _parent._syncPredicate(default(T), true, null);
                    if (locked) _parent._syncLock.Enter();
                    try {
                        _subscriber.OnCompleted();
                    } finally {
                        if (locked) _parent._syncLock.Exit();
                    }
                }

                public void OnError(Exception error)
                {
                    var locked = _parent._syncPredicate(default(T), false, error);
                    if (locked) _parent._syncLock.Enter();
                    try {
                        _subscriber.OnError(error);
                    } finally {
                        if (locked) _parent._syncLock.Exit();
                    }
                }

                public void OnNext(T value)
                {
                    var locked = _parent._syncPredicate(value, false, null);
                    if (locked) _parent._syncLock.Enter();
                    try {
                        _subscriber.OnNext(value);
                    } finally {
                        if (locked) _parent._syncLock.Exit();
                    }
                }
            }
        }
    }
}
