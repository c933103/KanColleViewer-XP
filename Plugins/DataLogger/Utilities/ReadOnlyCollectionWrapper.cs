using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Utilities
{
    public class ReadOnlyCollectionWrapper<T> : ICollection<T>
    {
        private ICollection<T> backend;

        public ReadOnlyCollectionWrapper(ICollection<T> backend)
        {
            this.backend = backend;
        }

        public int Count
        {
            get
            {
                return backend.Count;
            }
        }

        bool ICollection<T>.IsReadOnly { get { return true; } }

        public IEnumerator<T> GetEnumerator()
        {
            return backend.GetEnumerator();
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            backend.CopyTo(array, arrayIndex);
        }

        void ICollection<T>.Add(T item)
        {
            throw new InvalidOperationException();
        }

        void ICollection<T>.Clear()
        {
            throw new InvalidOperationException();
        }

        bool ICollection<T>.Contains(T item)
        {
            throw new InvalidOperationException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new InvalidOperationException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return backend.GetEnumerator();
        }
    }

    public class NotifyingReadOnlyCollectionWrapper<T> : ICollection<T>, INotifyCollectionChanged
    {
        private ICollection<T> backend;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public NotifyingReadOnlyCollectionWrapper(ICollection<T> backend, INotifyCollectionChanged subscriptionSource)
        {
            this.backend = backend;
            subscriptionSource.CollectionChanged += (o, e) => {
                Livet.DispatcherHelper.UIDispatcher.Invoke(new Action(() => {
                    var h = CollectionChanged;
                    if (h != null)
                        h(this, e);
                }));
            };
        }

        public int Count
        {
            get
            {
                return backend.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return backend.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return backend.GetEnumerator();
        }

        public void Add(T item)
        {
            throw new InvalidOperationException();
        }

        public void Clear()
        {
            throw new InvalidOperationException();
        }

        public bool Contains(T item)
        {
            return backend.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            backend.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            throw new InvalidOperationException();
        }
    }

    public class NotifyingSortedSet<T> : ISet<T>, INotifyCollectionChanged
    {
        private readonly SortedSet<T> _backend;

        public NotifyingSortedSet()
        {
            _backend = new SortedSet<T>();
        }

        public NotifyingSortedSet(IEnumerable<T> s)
        {
            _backend = new SortedSet<T>(s);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count
        {
            get
            {
                return ((ISet<T>)_backend).Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((ISet<T>)_backend).IsReadOnly;
            }
        }

        public void Clear()
        {
            _backend.Clear();
            RaiseCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            _backend.IntersectWith(other);
            RaiseCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        public SortedSet<T> GetViewBetween(T lowerValue, T upperValue)
        {
            throw new NotImplementedException();
        }

        public bool Add(T item)
        {
            var r = _backend.Add(item);
            if (r)
                RaiseCollectionChanged(NotifyCollectionChangedAction.Add, item, _backend.TakeWhile(x => _backend.Comparer.Compare(x, item) < 0).Count());
            return r;
        }

        private void RaiseCollectionChanged(NotifyCollectionChangedAction action)
        {
            Livet.DispatcherHelper.UIDispatcher.Invoke(new Action(() => {
                var h = CollectionChanged;
                if (h != null)
                    h(this, new NotifyCollectionChangedEventArgs(action));
            }));
        }

        private void RaiseCollectionChanged(NotifyCollectionChangedAction action, T item, int index)
        {
            Livet.DispatcherHelper.UIDispatcher.Invoke(new Action(() => {
                var h = CollectionChanged;
                if (h != null)
                    h(this, new NotifyCollectionChangedEventArgs(action, item, index));
            }));
        }

        public void UnionWith(IEnumerable<T> other)
        {
            ((ISet<T>)_backend).UnionWith(other);
            RaiseCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            ((ISet<T>)_backend).ExceptWith(other);
            RaiseCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            ((ISet<T>)_backend).SymmetricExceptWith(other);
            RaiseCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            return ((ISet<T>)_backend).IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            return ((ISet<T>)_backend).IsSupersetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            return ((ISet<T>)_backend).IsProperSupersetOf(other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            return ((ISet<T>)_backend).IsProperSubsetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            return ((ISet<T>)_backend).Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            return ((ISet<T>)_backend).SetEquals(other);
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public bool Contains(T item)
        {
            return ((ISet<T>)_backend).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((ISet<T>)_backend).CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var r = ((ISet<T>)_backend).Remove(item);
            if(r)
                RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, item, _backend.TakeWhile(x => _backend.Comparer.Compare(x, item) < 0).Count());
            return r;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((ISet<T>)_backend).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((ISet<T>)_backend).GetEnumerator();
        }
    }
}
