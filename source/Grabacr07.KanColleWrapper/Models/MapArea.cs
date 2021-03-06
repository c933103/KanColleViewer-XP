﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	public class MapArea : RawDataWrapper<kcsapi_mst_maparea>, IIdentifiable
	{
		public int Id { get; }

		public string Name { get; }

		public MapArea(kcsapi_mst_maparea maparea)
			: base(maparea)
		{
			this.Id = maparea.api_id;
			this.Name = maparea.api_name;
		}

		#region static members

	    public static MapArea Dummy { get; } = new MapArea(new kcsapi_mst_maparea()
		{
		    api_id = 0,
		    api_name = "？？？",
		});

	    #endregion
	}
}
