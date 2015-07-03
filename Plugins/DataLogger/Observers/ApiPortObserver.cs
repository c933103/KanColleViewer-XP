using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using LynLogger.Utilities;
using System;

namespace LynLogger.Observers
{
    class ApiPortObserver : IObserver<SvData<kcsapi_port>>
    {
        private Action _onPortAccess;
        public event Action OnPortAccess
        {
            add { _onPortAccess += value.MakeWeak(x => _onPortAccess -= x); }
            remove { }
        }


        public void OnNext(SvData<kcsapi_port> value)
        {
            try {
                if(!value.IsSuccess) return;
                DataStore.Store.SwitchMember(value.Data.api_basic.api_member_id);
                var ds = DataStore.Store.Current;

                ds.UpdateShips(value.Data.api_ship);
                ds.BasicInfo.Update(value.Data.api_material, true);
                ds.BasicInfo.Update(value.Data.api_basic);

                if (_onPortAccess != null) _onPortAccess();
            } catch(Exception e) {
                System.Diagnostics.Debugger.Break();
                System.Diagnostics.Trace.TraceError(e.ToString());
            }
        }

        public void OnCompleted() { return; }
        public void OnError(Exception error) { return; }
    }
}
