using System;
using System.Collections;
using System.Collections.Generic;
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
}
