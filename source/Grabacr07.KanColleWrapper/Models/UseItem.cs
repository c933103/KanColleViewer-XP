﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 消費アイテム表します。
	/// </summary>
	public class UseItem : RawDataWrapper<kcsapi_useitem>, IIdentifiable
	{
		public int Id => this.RawData.api_id;

		public string Name => this.RawData.api_name;

		public int Count => this.RawData.api_count;

		internal UseItem(kcsapi_useitem rawData) : base(rawData) { }

        internal void Update(kcsapi_useitem data)
        {
            UpdateRawData(data);
            RaisePropertyChanged(nameof(Name));
            RaisePropertyChanged(nameof(Count));
        }

		public override string ToString()
		{
			return $"ID = {this.Id}, Name = \"{this.Name}\", Count = {this.Count}";
		}
	}
}
