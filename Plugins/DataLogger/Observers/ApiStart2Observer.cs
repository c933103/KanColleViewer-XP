using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace LynLogger.Observers
{
    class ApiStart2Observer : IObserver<SvData<kcsapi_start2>>
    {
        public void OnCompleted()
        {
            return;
        }

        public void OnError(Exception error)
        {
            return;
        }

        public void OnNext(SvData<kcsapi_start2> value)
        {
            if(!value.IsSuccess) return;
        }
    }
}
