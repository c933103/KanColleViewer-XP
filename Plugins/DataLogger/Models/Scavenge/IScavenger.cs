using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Models.Scavenge
{
    public interface IScavenger
    {
        void Reset();
        bool ShouldKeep(object key, object value);
    }
}
