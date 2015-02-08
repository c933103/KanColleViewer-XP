using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLogger.Logger.Observers
{
    class APIPortObserver : IObserver<SvData<kcsapi_port>>
    {
        public void OnCompleted()
        {
            return;
        }

        public void OnError(Exception error)
        {
            return;
        }

        public void OnNext(SvData<kcsapi_port> value)
        {
            if(!value.IsSuccess) return;
            value.Data.api_ship
        }
    }
}
