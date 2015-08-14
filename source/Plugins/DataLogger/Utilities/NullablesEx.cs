using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Utilities
{
    public static class NullablesEx
    {
        public static double? SqrtNaT(double? n)
        {
            return n.HasValue ? Math.Sqrt(n.Value) : n;
        }
    }
}
