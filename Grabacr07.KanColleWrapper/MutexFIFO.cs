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
        private readonly int _spinIteration = 1000;

        private volatile int _grantedTicket = 1;
        private volatile int _acquireTicket = 0;
        private volatile int _acquiredThreadId = 0;
        private ThreadLocal<int> _ourTicket = new ThreadLocal<int>(() => 0);

        public MutexFIFO() : this(1000) { }
        public MutexFIFO(int spinIter) { _spinIteration = spinIter; }

        public void Enter()
        {
            if (_ourTicket.Value == _grantedTicket) goto _acquired;

            var ticket = _ourTicket.Value = Interlocked.Increment(ref _acquireTicket);
            for (int iter = 0; iter < _spinIteration; iter++) {
                if (_grantedTicket == ticket) goto _acquired;
            }

            lock (_syncRoot) {
                System.Diagnostics.Debug.WriteLine("[" + Thread.CurrentThread.ManagedThreadId + "] Unable to acquire mutex after " + _spinIteration + " spins, mutex currently held by thread " + _acquiredThreadId + ", current ticket is " + _grantedTicket + ", waiting for ticket " + ticket);
                while (true) {
                    if (_grantedTicket == ticket) goto _acquired;
                    Monitor.Wait(_syncRoot);
                }
            }

            _acquired:
            _acquiredThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public void Exit()
        {
            if (_ourTicket.Value != _grantedTicket)
                throw new SynchronizationLockException();

            _acquiredThreadId = 0;
            Interlocked.Increment(ref _grantedTicket);
            _ourTicket.Value = 0;

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
