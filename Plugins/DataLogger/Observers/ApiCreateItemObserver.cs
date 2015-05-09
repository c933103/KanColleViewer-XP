using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using LynLogger.DataStore.LogBook;
using LynLogger.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LynLogger.Observers
{
    class ApiCreateItemObserver : IObserver<SvData<kcsapi_createitem>>
    {
        private Action<CreateItemLog> _onItemCreate;
        public event Action<CreateItemLog> OnItemCreate
        {
            add { _onItemCreate += value.MakeWeak(x => _onItemCreate -= x); }
            remove { }
        }

        public void OnNext(SvData<kcsapi_createitem> value)
        {
            if(_onItemCreate == null) return;

            var req = value.Request;
            var res = value.Data;

            _onItemCreate(new CreateItemLog(int.Parse(req["api_item1"]), int.Parse(req["api_item2"]), int.Parse(req["api_item3"]), int.Parse(req["api_item4"]), res.api_slotitem_id, res.api_create_flag == 1 ? 1 : 0));
        }

        public void OnCompleted() { return; }
        public void OnError(Exception error) { return; }
    }
}
