using System;
using System.Collections.Generic;

namespace LynLogger.Utilities
{
    public class WeakEvent
    {
        private LinkedList<WeakReference<Action>> listeners = new LinkedList<WeakReference<Action>>();

        public void FireEvent(bool suppressExceptions = false)
        {
            lock (this) {
                LinkedListNode<WeakReference<Action>> node = listeners.First;
                Action a;
                LinkedList<Exception> exceptions = new LinkedList<Exception>();
                while(node != null) {
                    if(!node.Value.TryGetTarget(out a)) {
                        var prevNode = node.Previous;
                        node.List.Remove(node);
                        node = prevNode ?? listeners.First;
                    } else {
                        try {
                            a();
                        } catch (Exception e) {
                            exceptions.AddLast(e);
                        }
                    }
                    node = node?.Next;
                }
                if(!suppressExceptions && (exceptions.First != null)) {
                    throw new AggregateException(exceptions);
                }
            }
        }

        public void Add(Action listener)
        {
            lock (this) {
                LinkedListNode<WeakReference<Action>> node = listeners.First;
                Action a;
                while(node != null) {
                    if(!node.Value.TryGetTarget(out a)) {
                        var prevNode = node.Previous;
                        node.List.Remove(node);
                        node = prevNode ?? listeners.First;
                    }
                    node = node?.Next;
                }
                if(listener != null) {
                    listeners.AddLast(new WeakReference<Action>(listener));
                }
            }
        }

        public void RemoveLast(Action listener)
        {
            lock (this) {
                LinkedListNode<WeakReference<Action>> node = listeners.Last;
                Action a;
                bool removed = false;
                while(node != null) {
                    if(!node.Value.TryGetTarget(out a) || (!removed && a == listener)) {
                        var prevNode = node.Next;
                        node.List.Remove(node);
                        node = prevNode ?? listeners.Last;
                    }
                    node = node?.Previous;
                }
            }
        }
    }
    public class WeakEvent<T>
    {
        private LinkedList<WeakReference<Action<T>>> listeners = new LinkedList<WeakReference<Action<T>>>();

        public void FireEvent(T arg, bool suppressExceptions = false)
        {
            lock (this) {
                LinkedListNode<WeakReference<Action<T>>> node = listeners.First;
                Action<T> a;
                LinkedList<Exception> exceptions = new LinkedList<Exception>();
                while(node != null) {
                    if(!node.Value.TryGetTarget(out a)) {
                        var prevNode = node.Previous;
                        node.List.Remove(node);
                        node = prevNode ?? listeners.First;
                    } else {
                        try {
                            a(arg);
                        } catch (Exception e) {
                            exceptions.AddLast(e);
                        }
                    }
                    node = node?.Next;
                }
                if(!suppressExceptions && (exceptions.First != null)) {
                    throw new AggregateException(exceptions);
                }
            }
        }

        public void Add(Action<T> listener)
        {
            lock (this) {
                LinkedListNode<WeakReference<Action<T>>> node = listeners.First;
                Action<T> a;
                while(node != null) {
                    if(!node.Value.TryGetTarget(out a)) {
                        var prevNode = node.Previous;
                        node.List.Remove(node);
                        node = prevNode ?? listeners.First;
                    }
                    node = node?.Next;
                }
                if(listener != null) {
                    listeners.AddLast(new WeakReference<Action<T>>(listener));
                }
            }
        }

        public void RemoveLast(Action<T> listener)
        {
            lock (this) {
                LinkedListNode<WeakReference<Action<T>>> node = listeners.Last;
                Action<T> a;
                bool removed = false;
                while(node != null) {
                    if(!node.Value.TryGetTarget(out a) || (!removed && a == listener)) {
                        var prevNode = node.Next;
                        node.List.Remove(node);
                        node = prevNode ?? listeners.Last;
                    }
                    node = node?.Previous;
                }
            }
        }
    }
    public class WeakEvent<T1,T2>
    {
        private LinkedList<WeakReference<Action<T1, T2>>> listeners = new LinkedList<WeakReference<Action<T1, T2>>>();

        public void FireEvent(T1 arg1, T2 arg2, bool suppressExceptions = false)
        {
            lock (this) {
                LinkedListNode<WeakReference<Action<T1, T2>>> node = listeners.First;
                Action<T1, T2> a;
                LinkedList<Exception> exceptions = new LinkedList<Exception>();
                while(node != null) {
                    if(!node.Value.TryGetTarget(out a)) {
                        var prevNode = node.Previous;
                        node.List.Remove(node);
                        node = prevNode ?? listeners.First;
                    } else {
                        try {
                            a(arg1, arg2);
                        } catch (Exception e) {
                            exceptions.AddLast(e);
                        }
                    }
                    node = node?.Next;
                }
                if(!suppressExceptions && (exceptions.First != null)) {
                    throw new AggregateException(exceptions);
                }
            }
        }

        public void Add(Action<T1, T2> listener)
        {
            lock (this) {
                LinkedListNode<WeakReference<Action<T1, T2>>> node = listeners.First;
                Action<T1, T2> a;
                while(node != null) {
                    if(!node.Value.TryGetTarget(out a)) {
                        var prevNode = node.Previous;
                        node.List.Remove(node);
                        node = prevNode ?? listeners.First;
                    }
                    node = node?.Next;
                }
                if(listener != null) {
                    listeners.AddLast(new WeakReference<Action<T1, T2>>(listener));
                }
            }
        }

        public void RemoveLast(Action<T1, T2> listener)
        {
            lock (this) {
                LinkedListNode<WeakReference<Action<T1, T2>>> node = listeners.Last;
                Action<T1, T2> a;
                bool removed = false;
                while(node != null) {
                    if(!node.Value.TryGetTarget(out a) || (!removed && a == listener)) {
                        var prevNode = node.Next;
                        node.List.Remove(node);
                        node = prevNode ?? listeners.Last;
                    }
                    node = node?.Previous;
                }
            }
        }
    }
}
