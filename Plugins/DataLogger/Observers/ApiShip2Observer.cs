using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using System;

namespace LynLogger.Observers
{
    class ApiShip2Observer : IObserver<SvData<kcsapi_ship2[]>>
    {
        public void OnNext(SvData<kcsapi_ship2[]> value)
        {
            try {
                if(!value.IsSuccess) return;
                DataStore.Store.Current.UpdateShips(value.Data);
            } catch(Exception e) {
                System.Diagnostics.Debugger.Break();
                System.Diagnostics.Trace.TraceError(e.ToString());
            }
        }

        public void OnCompleted() { return; }
        public void OnError(Exception error) { return; }
    }
}
