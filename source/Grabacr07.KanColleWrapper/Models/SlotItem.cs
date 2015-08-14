using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	public class SlotItem : RawDataWrapper<kcsapi_slotitem>, IIdentifiable
	{
		public int Id => this.RawData.api_id;

        private SlotItemInfo _info;
		public SlotItemInfo Info
        {
            get { return _info; }
            private set
            {
                if (_info == value) return;
                _info = value;
                RaisePropertyChanged();
            }
        }

		public int Level => this.RawData.api_alv == 0 ? this.RawData.api_level : this.RawData.api_alv;

		public string LevelText {
            get
            {
                var level = this.RawData.api_alv;
                if(level == 0) {
                    level = this.RawData.api_level;
                    if(level >= 10) return "★max";
                }
                return level > 0 ? "★+" + level : "";
            }
        }

		public string NameWithLevel => $"{this.Info.Name}{(this.Level >= 1 ? (" " + this.LevelText) : "")}";

		internal SlotItem(kcsapi_slotitem rawData)
			: base(rawData)
		{
			this.Info = KanColleClient.Current.Master.SlotItems[this.RawData.api_slotitem_id] ?? SlotItemInfo.Dummy;
		}

        internal void Update(kcsapi_slotitem data)
        {
            var oldLevel = Level;

            UpdateRawData(data);
            this.Info = KanColleClient.Current.Master.SlotItems[this.RawData.api_slotitem_id] ?? SlotItemInfo.Dummy;

            if(Level != oldLevel) {
                this.RaiseLevelPropertyChanged();
            }
        }

		public void Remodel(int level, int masterId)
		{
			this.RawData.api_level = level;
			this.Info = KanColleClient.Current.Master.SlotItems[masterId] ?? SlotItemInfo.Dummy;

            RaiseLevelPropertyChanged();
        }

		public override string ToString()
		{
			return $"ID = {this.Id}, Name = \"{this.Info.Name}\", Level = {this.Level}";
		}

        private void RaiseLevelPropertyChanged()
        {
            RaisePropertyChanged(nameof(Level));
            RaisePropertyChanged(nameof(LevelText));
            RaisePropertyChanged(nameof(NameWithLevel));
        }
	}
}
