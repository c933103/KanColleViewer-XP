using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger
{
    static class ArrayExtensions
    {
        public static T Get<T>(this T[] arr, int i) { return arr[i]; }
    }
}
