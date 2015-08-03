using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Composition
{
	public class ToolViewModel : ViewModel
    {
		private readonly ITool tool;
        private string _name;
        private object _view;


		public ToolViewModel(ITool tool)
		{
			this.tool = tool;
		}

		public string Name
		{
			get { return this.tool.Name; }
		}

		public object View
		{
				get { return _view ?? (_view = tool.View); }
		}
	}
}
