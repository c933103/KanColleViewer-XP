using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Observers
{
    class ApiMemberBasicObserver : IObserver<SvData<kcsapi_basic>>
    {
        public void OnCompleted()
        {
            return;
        }

        public void OnError(Exception error)
        {
            return;
        }

        public void OnNext(SvData<kcsapi_basic> value)
        {
            try {
                if(!value.IsSuccess) return;
                DataStore.SwitchMember(value.Data.api_member_id);
                DataStore.Instance.BasicInfo.Update(value.Data);
            } catch(Exception e) {
                System.Diagnostics.Debugger.Break();
                System.Diagnostics.Trace.TraceError(e.ToString());
            }
        }
    }
}
