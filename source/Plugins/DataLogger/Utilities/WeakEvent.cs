using System;
using System.Collections.Generic;

namespace LynLogger.Utilities
{
    public interface IWeakAction
    {
        Action Handler { get; }
        void Invoke();
    }

    public interface IWeakAction<TIn>
    {
        Action<TIn> Handler { get; }
        void Invoke(TIn arg1);
    }

    public interface IWeakAction<TIn1, TIn2>
    {
        Action<TIn1, TIn2> Handler { get; }
        void Invoke(TIn1 arg1, TIn2 arg2);
    }

    public class WeakAction<TTarget> : IWeakAction
        where TTarget : class
    {
        private delegate void UnboundHandler(TTarget @this);

        private readonly WeakReference _target;
        private readonly UnboundHandler _unboundHandler;
        private readonly Action<Action> _targetDead;

        public Action Handler { get; }

        public WeakAction(Action a)
        {
            _target = new WeakReference((TTarget)a.Target);
            _unboundHandler = (UnboundHandler)Delegate.CreateDelegate(typeof(UnboundHandler), null, a.Method);
            Handler = Invoke;
        }

        public WeakAction(Action a, Action<Action> targetDead) : this(a)
        {
            _targetDead = targetDead;
        }

        public void Invoke()
        {
            TTarget target = _target.Target as TTarget;
            if (target != null) {
                _unboundHandler(target);
            } else {
                if(_targetDead != null) {
                    _targetDead(Handler);
                }
            }
        }

        public static implicit operator Action(WeakAction<TTarget> t)
        {
            return t.Handler;
        }
    }

    public class WeakAction<TTarget, TIn> : IWeakAction<TIn>
        where TTarget : class
    {
        private delegate void UnboundHandler(TTarget @this, TIn arg1);

        private readonly WeakReference _target;
        private readonly UnboundHandler _unboundHandler;
        private readonly Action<Action<TIn>> _targetDead;

        public Action<TIn> Handler { get; }

        public WeakAction(Action<TIn> a)
        {
            _target = new WeakReference((TTarget)a.Target);
            _unboundHandler = (UnboundHandler)Delegate.CreateDelegate(typeof(UnboundHandler), null, a.Method);
            Handler = Invoke;
        }

        public WeakAction(Action<TIn> a, Action<Action<TIn>> targetDead) : this(a)
        {
            _targetDead = targetDead;
        }

        public void Invoke(TIn arg)
        {
            TTarget target = _target.Target as TTarget;
            if(target != null) {
                _unboundHandler(target, arg);
            } else {
                if(_targetDead != null) {
                    _targetDead(Handler);
                }
            }
        }

        public static implicit operator Action<TIn>(WeakAction<TTarget, TIn> t)
        {
            return t.Handler;
        }
    }

    public class WeakAction<TTarget, TIn1, TIn2> : IWeakAction<TIn1, TIn2>
        where TTarget : class
    {
        private delegate void UnboundHandler(TTarget @this, TIn1 arg1, TIn2 arg2);

        private readonly WeakReference _target;
        private readonly UnboundHandler _unboundHandler;
        private readonly Action<Action<TIn1, TIn2>> _targetDead;

        public Action<TIn1, TIn2> Handler { get; }

        public WeakAction(Action<TIn1, TIn2> a)
        {
            _target = new WeakReference((TTarget)a.Target);
            _unboundHandler = (UnboundHandler)Delegate.CreateDelegate(typeof(UnboundHandler), null, a.Method);
            Handler = Invoke;
        }

        public WeakAction(Action<TIn1, TIn2> a, Action<Action<TIn1, TIn2>> targetDead) : this(a)
        {
            _targetDead = targetDead;
        }

        public void Invoke(TIn1 arg1, TIn2 arg2)
        {
            TTarget target = _target.Target as TTarget;
            if (target != null) {
                _unboundHandler(target, arg1, arg2);
            } else {
                if(_targetDead != null) {
                    _targetDead(Handler);
                }
            }
        }

        public static implicit operator Action<TIn1, TIn2>(WeakAction<TTarget, TIn1, TIn2> t)
        {
            return t.Handler;
        }
    }

    public static class WeakAction
    {
        public static Action MakeWeak(this Action callback, Action<Action> targetDead)
        {
            if(callback == null) throw new ArgumentNullException();
            if(callback.Method.IsStatic) return callback;
            if(callback.Target == null) return callback;

            var weakActionType = typeof(WeakAction<>).MakeGenericType(callback.Target.GetType());
            var weakActionConstructor = weakActionType.GetConstructor(new Type[] { typeof(Action), typeof(Action<Action>) });
            return ((IWeakAction)weakActionConstructor.Invoke(new object[] { callback, targetDead })).Handler;
        }

        public static Action<TIn> MakeWeak<TIn>(this Action<TIn> callback, Action<Action<TIn>> targetDead)
        {
            if(callback == null) throw new ArgumentNullException();
            if(callback.Method.IsStatic) return callback;
            if(callback.Target == null) return callback;

            var weakActionType = typeof(WeakAction<,>).MakeGenericType(callback.Target.GetType(), typeof(TIn));
            var weakActionConstructor = weakActionType.GetConstructor(new Type[] { typeof(Action<TIn>), typeof(Action<Action<TIn>>) });
            return ((IWeakAction<TIn>)weakActionConstructor.Invoke(new object[] { callback, targetDead })).Handler;
        }

        public static Action<TIn1, TIn2> MakeWeak<TIn1, TIn2>(this Action<TIn1, TIn2> callback, Action<Action<TIn1, TIn2>> targetDead)
        {
            if(callback == null) throw new ArgumentNullException();
            if(callback.Method.IsStatic) return callback;
            if(callback.Target == null) return callback;

            var weakActionType = typeof(WeakAction<,,>).MakeGenericType(callback.Target.GetType(), typeof(TIn1), typeof(TIn2));
            var weakActionConstructor = weakActionType.GetConstructor(new Type[] { typeof(Action<TIn1, TIn2>), typeof(Action<Action<TIn1, TIn2>>) });
            return ((IWeakAction<TIn1, TIn2>)weakActionConstructor.Invoke(new object[] { callback, targetDead })).Handler;
        }
    }
}
