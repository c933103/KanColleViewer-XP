using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using LynLogger.Models.Battling;
using LynLogger.Utilities;
using System;

namespace LynLogger.Observers
{
    class ApiBattleResultObserver : IObserver<SvData<kcsapi_battleresult>>
    {
        private Action<BattleResult> _onBattleResult;
        public event Action<BattleResult> OnBattleResult
        {
            add { _onBattleResult += value.MakeWeak(x => _onBattleResult -= x); }
            remove { }
        }

        public void OnNext(SvData<kcsapi_battleresult> value)
        {
            //value.Data
        }

        public void OnCompleted() { }
        public void OnError(Exception error) { }
    }
}
