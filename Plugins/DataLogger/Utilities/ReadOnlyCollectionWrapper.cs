using System;
using System.Collections;
using System.Collections.Generic;
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

        public int Count
        {
            get
            {
                return backend.Count;
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
    }
}
