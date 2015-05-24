using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleWrapper
{
	/// <summary>
	/// 整数値の ID をキーとして使用する、艦これマスター データ用のテーブルを定義します。
	/// </summary>
	/// <typeparam name="TValue">マスター データの型。</typeparam>
	public class MasterTable<TValue> : IDictionary<int, TValue> where TValue : class, IIdentifiable
	{
		private readonly IDictionary<int, TValue> dictionary;

		/// <summary>
		/// テーブルから指定した ID の要素を取得します。ID が存在しない場合は null を返します。
		/// </summary>
		public TValue this[int key]
		{
			get { return this.dictionary.ContainsKey(key) ? this.dictionary[key] : null; }
            set { throw new InvalidOperationException(); }
        }

		public MasterTable() : this(new List<TValue>()) { }

		public MasterTable(IEnumerable<TValue> source)
		{
			this.dictionary = source.ToDictionary(x => x.Id);
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

        bool ICollection<KeyValuePair<int, TValue>>.Contains(KeyValuePair<int, TValue> item)
        {
            return dictionary.Contains(item);
        }

        void ICollection<KeyValuePair<int, TValue>>.CopyTo(KeyValuePair<int, TValue>[] array, int arrayIndex)
        {
            dictionary.CopyTo(array, arrayIndex);
        }

        public ICollection<int> Keys
		{
			get { return this.dictionary.Keys; }
		}

		public ICollection<TValue> Values
		{
			get { return this.dictionary.Values; }
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
    }
}
