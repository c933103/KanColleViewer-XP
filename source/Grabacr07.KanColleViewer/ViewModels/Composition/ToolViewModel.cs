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
        private object _view;


		public string Name => this.tool.Name;

		public object View => _view ?? (_view = tool.View);

		public ToolViewModel(ITool tool)
		{
			this.tool = tool;
		}
	}
}
