﻿using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using System;

namespace LynLogger.Observers
{
    class ApiBattleResultObserver : IObserver<SvData<kcsapi_battleresult>>
    {
        public void OnNext(SvData<kcsapi_battleresult> value)
        {
            //value.Data
        }

        public void OnCompleted() { return; }
        public void OnError(Exception error) { return; }
    }
}
