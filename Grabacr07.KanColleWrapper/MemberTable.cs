using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using System.Collections.Specialized;

namespace Grabacr07.KanColleWrapper
{
	/// <summary>
	/// 整数値の ID をキーとして使用する、艦これユーザー データ用のテーブルを定義します。
	/// </summary>
	/// <typeparam name="TValue">ユーザー データの型。</typeparam>
	public class MemberTable<TValue> : IReadOnlyDictionary<int, TValue>, INotifyCollectionChanged where TValue : class, IIdentifiable
	{
		private readonly SortedList<int, TValue> dictionary;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        
        /// <summary>
        /// テーブルから指定した ID の要素を取得します。ID が存在しない場合は null を返します。
        /// </summary>
        public TValue this[int key]
		{
			get { return this.dictionary.ContainsKey(key) ? this.dictionary[key] : null; }
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
			this.dictionary.Remove(value.Id);
            RaiseCollectionItemRemoved(value);
        }

		internal void Remove(int id)
		{
            TValue obj;
            if(dictionary.TryGetValue(id, out obj)) {
                this.dictionary.Remove(id);
                RaiseCollectionItemRemoved(obj);
            }
		}

		#region IReadOnlyDictionary<TK, TV> members

		public IEnumerator<KeyValuePair<int, TValue>> GetEnumerator()
		{
			return this.dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public int Count
		{
			get { return this.dictionary.Count; }
		}

		public bool ContainsKey(int key)
		{
			return this.dictionary.ContainsKey(key);
		}

		public bool TryGetValue(int key, out TValue value)
		{
			return this.dictionary.TryGetValue(key, out value);
		}

		public IEnumerable<int> Keys
		{
			get { return this.dictionary.Keys; }
		}

		public IEnumerable<TValue> Values
		{
			get { return this.dictionary.Values; }
		}

        #endregion

        private void RaiseCollectionItemRemoved(TValue obj)
        {
            var handler = CollectionChanged;
            if (handler == null) return;

            var index = dictionary.Keys.BinarySearchEqualOrHigher(obj.Id);
            var ev = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, obj, index);

            DoRaiseCollectionChanged(handler, ev);
        }

        private void RaiseCollectionItemAdded(TValue obj)
        {
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
    }
}
