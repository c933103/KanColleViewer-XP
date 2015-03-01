using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Models.Scavenge
{
    interface IRampUpScavenger : IScavenger
    {
        void RampUp(object key, object value);
    }
}
