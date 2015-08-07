using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Grabacr07.KanColleWrapper
{
	/// <summary>
	/// 整数値の ID をキーとして使用する、艦これユーザー データ用のテーブルを定義します。
	/// </summary>
	/// <typeparam name="TValue">ユーザー データの型。</typeparam>
	public class MemberTable<TValue> : IDictionary<int, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
        where TValue : class, IIdentifiable
	{
		private readonly SortedList<int, TValue> dictionary;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// テーブルから指定した ID の要素を取得します。ID が存在しない場合は null を返します。
        /// </summary>
        public TValue this[int key]
		{
			get { return this.dictionary.ContainsKey(key) ? this.dictionary[key] : null; }
            set { throw new InvalidOperationException(); }
        }
        
		public MemberTable() : this(null) { }

		public MemberTable(IEnumerable<TValue> source)
		{
			this.dictionary = source?.ToSortedList(x => x.Id) ?? new SortedList<int, TValue>();
		}

        internal bool UpdateValueRange<TIn>(IEnumerable<TIn> source, Func<TIn, int> idSelector, Func<TIn, TValue> valueFactory, Action<TValue, TIn> updateMethod = null, bool doDispose = false, Action<TValue> disposeMethod = null)
        {
            var collectionReset = false;
            HashSet<int> removeIds = null;
            if (doDispose) removeIds = new HashSet<int>(dictionary.Keys);

            foreach (var value in source) {
                var id = idSelector(value);
                TValue obj;
                bool keyExist = dictionary.TryGetValue(id, out obj);

                if (doDispose) removeIds.Remove(id);
                if (updateMethod != null && keyExist) {
                    updateMethod(obj, value);
                } else {
                    var val = valueFactory(value);
                    System.Diagnostics.Debug.Assert(val.Id == id);
                    dictionary[id] = val;
                    if (keyExist) {
                        RaiseCollectionItemReplaced(obj, val);
                    } else {
                        RaiseCollectionItemAdded(val);
                    }
                    collectionReset = true;
                }
            }

            if (doDispose) {
                foreach (var id in removeIds) {
                    var obj = dictionary[id];
                    disposeMethod?.Invoke(obj);
                    dictionary.Remove(id);
                    RaiseCollectionItemRemoved(obj);
                    collectionReset = true;
                }
            }

            return collectionReset;
        }

		internal void Add(TValue value)
		{
			this.dictionary.Add(value.Id, value);
            RaiseCollectionItemAdded(value);
        }

		internal void Remove(TValue value)
		{
            if (this.dictionary.Remove(value.Id)) {
                RaiseCollectionItemRemoved(value);
            }
        }

		internal void Remove(int id)
		{
            TValue obj;
            if(dictionary.TryGetValue(id, out obj)) {
                this.dictionary.Remove(id);
                RaiseCollectionItemRemoved(obj);
            }
		}

		#region IDictionary<TK, TV> members

		public IEnumerator<KeyValuePair<int, TValue>> GetEnumerator()
		{
			return this.dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public int Count => this.dictionary.Count;

		public bool ContainsKey(int key)
		{
			return this.dictionary.ContainsKey(key);
		}

		public bool TryGetValue(int key, out TValue value)
		{
			return this.dictionary.TryGetValue(key, out value);
		}

		public ICollection<int> Keys => this.dictionary.Keys;

		public ICollection<TValue> Values => this.dictionary.Values;

        bool ICollection<KeyValuePair<int, TValue>>.Contains(KeyValuePair<int, TValue> item)
        {
            return dictionary.Contains(item);
        }

        void ICollection<KeyValuePair<int, TValue>>.CopyTo(KeyValuePair<int, TValue>[] array, int arrayIndex)
        {
            (dictionary as ICollection<KeyValuePair<int, TValue>>).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<int, TValue>>.IsReadOnly { get { return true; } }

        void IDictionary<int, TValue>.Add(int key, TValue value)
        {
            throw new InvalidOperationException();
        }

        bool IDictionary<int, TValue>.Remove(int key)
        {
            throw new InvalidOperationException();
        }

        void ICollection<KeyValuePair<int, TValue>>.Add(KeyValuePair<int, TValue> item)
        {
            throw new InvalidOperationException();
        }

        void ICollection<KeyValuePair<int, TValue>>.Clear()
        {
            throw new InvalidOperationException();
        }

        bool ICollection<KeyValuePair<int, TValue>>.Remove(KeyValuePair<int, TValue> item)
        {
            throw new InvalidOperationException();
        }

        #endregion

        private void RaiseCollectionItemRemoved(TValue obj)
        {
            DoRaisePropertyChanged();

            var handler = CollectionChanged;
            if (handler == null) return;

            var index = dictionary.Keys.BinarySearchEqualOrHigher(obj.Id);
            var ev = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, obj, index);

            DoRaiseCollectionChanged(handler, ev);
        }

        private void RaiseCollectionItemAdded(TValue obj)
        {
            DoRaisePropertyChanged();

            var handler = CollectionChanged;
            if (handler == null) return;

            var index = dictionary.Keys.BinarySearchEqualOrHigher(obj.Id);
            var ev = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, obj, index);

            DoRaiseCollectionChanged(handler, ev);
        }

        private void RaiseCollectionItemReplaced(TValue objOld, TValue objNew)
        {
            System.Diagnostics.Debug.Assert(objOld.Id == objNew.Id);

            var handler = CollectionChanged;
            if (handler == null) return;

            var index = dictionary.Keys.BinarySearchEqualOrHigher(objNew.Id);
            var ev = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, objNew, objOld, index);

            DoRaiseCollectionChanged(handler, ev);
        }

        private volatile System.Windows.Threading.DispatcherOperation _lastOp;
        private void DoRaiseCollectionChanged(NotifyCollectionChangedEventHandler handler, NotifyCollectionChangedEventArgs ev)
        {
            //var lastOp = _lastOp;
            //while (true) {
                _lastOp?.Wait();
                //if (object.ReferenceEquals(System.Threading.Interlocked.CompareExchange(ref _lastOp, null, lastOp), lastOp)) {
                    _lastOp = System.Windows.Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.DataBind, handler, this, ev);
                    return;
                //}
                //lastOp = _lastOp;
            //}
        }

        private static readonly PropertyChangedEventArgs _countPropertyChanged = new PropertyChangedEventArgs(nameof(Count));
        private void DoRaisePropertyChanged()
        {
            PropertyChanged?.Invoke(this, _countPropertyChanged);
        }
    }
}
