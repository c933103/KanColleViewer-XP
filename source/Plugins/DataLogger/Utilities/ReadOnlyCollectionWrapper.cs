using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Utilities
{
    public class ReadOnlyCollectionWrapper<T> : IReadOnlyCollection<T>
    {
        private ICollection<T> backend;

        public ReadOnlyCollectionWrapper(ICollection<T> backend)
        {
            this.backend = backend;
        }

        public int Count => backend.Count;
        public IEnumerator<T> GetEnumerator() => backend.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => backend.GetEnumerator();
    }

    public class NotifyingReadOnlyCollectionWrapper<T> : IReadOnlyCollection<T>, INotifyCollectionChanged
    {
        private ICollection<T> backend;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public NotifyingReadOnlyCollectionWrapper(ICollection<T> backend, INotifyCollectionChanged subscriptionSource)
        {
            this.backend = backend;
            subscriptionSource.CollectionChanged += (o, e) => {
                CollectionChanged?.Invoke(this, e);
            };
        }

        public int Count => backend.Count;
        public IEnumerator<T> GetEnumerator() => backend.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => backend.GetEnumerator();
    }

    public class NotifyingSortedSet<T> : ISet<T>, INotifyCollectionChanged
    {
        private readonly SortedSet<T> _backend;

        public NotifyingSortedSet() : this(new SortedSet<T>()) { }
        public NotifyingSortedSet(IEnumerable<T> s)
        {
            _backend = new SortedSet<T>(s);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count => _backend.Count;
        public bool IsReadOnly => ((ISet<T>)_backend).IsReadOnly;

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
                RaiseCollectionChanged(NotifyCollectionChangedAction.Add, item, _backend.GetViewBetween(_backend.Min, item).Count - 1);
            return r;
        }

        private void RaiseCollectionChanged(NotifyCollectionChangedAction action)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action));
        }

        private void RaiseCollectionChanged(NotifyCollectionChangedAction action, T item, int index)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action));
        }

        public void UnionWith(IEnumerable<T> other)
        {
            _backend.UnionWith(other);
            RaiseCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            _backend.ExceptWith(other);
            RaiseCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            _backend.SymmetricExceptWith(other);
            RaiseCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        public bool Remove(T item)
        {
            var r = _backend.Remove(item);
            if (r)
                RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, item, _backend.GetViewBetween(_backend.Min, item).Count);
            return r;
        }

        void ICollection<T>.Add(T item) => Add(item);
        public bool Contains(T item) => _backend.Contains(item);
        public IEnumerator<T> GetEnumerator() => _backend.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _backend.GetEnumerator();
        public bool Overlaps(IEnumerable<T> other) => _backend.Overlaps(other);
        public bool SetEquals(IEnumerable<T> other) => _backend.SetEquals(other);
        public bool IsSubsetOf(IEnumerable<T> other) => _backend.IsSubsetOf(other);
        public bool IsSupersetOf(IEnumerable<T> other) => _backend.IsSupersetOf(other);
        public void CopyTo(T[] array, int arrayIndex) => _backend.CopyTo(array, arrayIndex);
        public bool IsProperSubsetOf(IEnumerable<T> other) => _backend.IsProperSubsetOf(other);
        public bool IsProperSupersetOf(IEnumerable<T> other) => _backend.IsProperSupersetOf(other);
    }
}
