﻿using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Observers
{
    class ApiShip2Observer : IObserver<SvData<kcsapi_ship2[]>>
    {
        public void OnCompleted()
        {
            return;
        }

        public void OnError(Exception error)
        {
            return;
        }

        public void OnNext(SvData<kcsapi_ship2[]> value)
        {
            try {
                if(!value.IsSuccess) return;
                Models.DataStore.Instance.UpdateShips(value.Data);
            } catch(Exception e) {
                System.Diagnostics.Debugger.Break();
                System.Diagnostics.Trace.TraceError(e.ToString());
            }
        }
    }
}