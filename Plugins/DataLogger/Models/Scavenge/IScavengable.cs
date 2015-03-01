using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Models.Scavenge
{
    interface IScavengable
    {
        int Scavenge(IScavenger sc, KeyValuePair<Type, Type>[] targetTypes);
    }
}
