using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper
{
	public interface IProxySettings
	{
        IDictionary<int, ProxyRule> Rules { get; }
        ProxyRule[] CompiledRules { get; set; }
	}
}
