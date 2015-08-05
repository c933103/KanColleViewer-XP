﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.Desktop.Metro.Controls;
using Grabacr07.KanColleViewer.Composition;

namespace Grabacr07.KanColleViewer.Views.Settings
{
	public abstract class FunctionLink<T> : Link
	{
		public T Function { get; set; }
	}

	public class SettingsText : FunctionLink<ISettings>
	{
		public override void Click()
		{
		}
	}

	public class NotifierText : FunctionLink<INotifier>
	{
		public Action TestMethod { get; set; }

		public override void Click()
		{
			this.TestMethod?.Invoke();
		}
	}

	public class RequestNotifyText : FunctionLink<IRequestNotify>
	{
		public override void Click()
		{
		}
	}

	public class ToolText : FunctionLink<ITool>
	{
		public override void Click()
		{
		}
	}
}
