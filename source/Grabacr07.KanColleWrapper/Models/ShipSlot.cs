using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Livet;

namespace Grabacr07.KanColleWrapper.Models
{
	public class ShipSlot : NotificationObjectEx
	{
        private SlotItem _item;
		public SlotItem Item
        {
            get { return _item; }
            private set
            {
                if (_item == value) return;
                _item = value;
                RaisePropertyChanged(nameof(Item));
                RaisePropertyChanged(nameof(Equipped));
            }
        }

        private int _max;
		public int Maximum
        {
            get { return _max; }
            private set
            {
                if (_max == value) return;
                _max = value;
                RaisePropertyChanged();
            }
        }

		public bool Equipped => this.Item != null;

		#region Current 変更通知プロパティ

		private int _Current;

		public int Current
		{
			get { return this._Current; }
			set
			{
				if (this._Current != value)
				{
					this._Current = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public ShipSlot(SlotItem item, int maximum, int current)
		{
            Update(item, maximum, current);
		}

        internal void Update(SlotItem item, int maximum, int current)
        {
            this.Item = item;
            this.Maximum = maximum;
            this.Current = current;
        }
    }
}