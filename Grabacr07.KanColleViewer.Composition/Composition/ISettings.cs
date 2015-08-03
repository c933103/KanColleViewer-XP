﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Composition
{
	/// <summary>
	/// プラグインの設定画面を呼び出すためのメンバーを公開します。
	/// この型をコントラクトとしてエクスポートするとき、1 つの GUID につき 1 つまでしか使用されないことに注意してください。
	/// </summary>
	public interface ISettings
	{
		/// <summary>
		/// [設定] タブ内に表示されるプラグイン設定 UI のルート要素を取得します。
		/// </summary>
		object View { get; }
	}
}
